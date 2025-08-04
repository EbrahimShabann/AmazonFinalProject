using Final_project.Helpers;
using Final_project.Models;
using Microsoft.EntityFrameworkCore;


namespace Final_project.Services.DeviceService
{
    public class EnhancedDeviceService : IDeviceService
    {
        private readonly AmazonDBContext _context;
        private readonly ILogger<EnhancedDeviceService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnhancedDeviceService(AmazonDBContext context, ILogger<EnhancedDeviceService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateDeviceFingerprint(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
            var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();
            var acceptCharset = context.Request.Headers["Accept-Charset"].ToString();
            var ipAddress = GetClientIpAddress(context);

            // Get screen resolution if available (would come from JavaScript)
            var screenResolution = context.Request.Query["screen"].ToString();
            var timezone = context.Request.Query["timezone"].ToString();

            var fingerprint = $"{userAgent}|{acceptLanguage}|{acceptEncoding}|{acceptCharset}|{ipAddress}|{screenResolution}|{timezone}";

            return SecurityHelper.HashDeviceFingerprint(fingerprint);
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP first
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public async Task<UserDevice> GetOrCreateDeviceAsync(string userId, HttpContext context)
        {
            var fingerprint = GenerateDeviceFingerprint(context);
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var ipAddress = GetClientIpAddress(context);

            var device = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceFingerprint == fingerprint);

            if (device == null)
            {
                device = new UserDevice
                {
                    UserId = userId,
                    DeviceFingerprint = fingerprint,
                    UserAgent = userAgent,
                    IpAddress = ipAddress,
                    FirstSeen = DateTime.UtcNow,
                    LastSeen = DateTime.UtcNow,
                    IsTrusted = false,
                    DeviceName = GetDetailedDeviceName(userAgent),
                    Browser = GetBrowserName(userAgent) ?? "Unknown",
                    OperatingSystem = GetOperatingSystem(userAgent) ?? "Unknown",
                    DeviceType = GetDeviceType(userAgent),
                    Location= "Unknown",



                };

                _context.UserDevices.Add(device);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New device registered for user {userId}: {device.DeviceName} from {ipAddress}");
            }
            else
            {
                // Update device information
                device.LastSeen = DateTime.UtcNow;
                if (device.IpAddress != ipAddress)
                {
                    _logger.LogInformation($"IP address changed for device {device.Id}: {device.IpAddress} -> {ipAddress}");
                    device.IpAddress = ipAddress;
                }
                await _context.SaveChangesAsync();
            }

            return device;
        }

        public async Task<bool> IsNewDeviceAsync(string userId, HttpContext context)
        {
            var fingerprint = GenerateDeviceFingerprint(context);

            var existingDevice = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId &&
                                        d.DeviceFingerprint == fingerprint &&
                                        d.IsTrusted);

            return existingDevice == null;
        }

        public async Task MarkDeviceAsTrustedAsync(int deviceId)
        {
            var device = await _context.UserDevices.FindAsync(deviceId);
            if (device != null)
            {
                device.IsTrusted = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Device {deviceId} marked as trusted for user {device.UserId}");
            }
        }

        private string GetDetailedDeviceName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown Device";

            var ua = userAgent.ToLower();

            // Mobile devices
            if (ua.Contains("mobile"))
            {
                if (ua.Contains("iphone")) return "iPhone";
                if (ua.Contains("android")) return "Android Phone";
                if (ua.Contains("windows phone")) return "Windows Phone";
                return "Mobile Device";
            }

            // Tablets
            if (ua.Contains("tablet") || ua.Contains("ipad"))
            {
                if (ua.Contains("ipad")) return "iPad";
                if (ua.Contains("android")) return "Android Tablet";
                return "Tablet";
            }

            // Desktop browsers
            if (ua.Contains("chrome")) return "Chrome Browser";
            if (ua.Contains("firefox")) return "Firefox Browser";
            if (ua.Contains("safari") && !ua.Contains("chrome")) return "Safari Browser";
            if (ua.Contains("edge")) return "Edge Browser";
            if (ua.Contains("opera")) return "Opera Browser";

            // Operating Systems
            if (ua.Contains("windows")) return "Windows Desktop";
            if (ua.Contains("macintosh") || ua.Contains("mac os")) return "Mac Desktop";
            if (ua.Contains("linux")) return "Linux Desktop";

            return "Desktop Computer";
        }

        public async Task<List<UserDevice>> GetSuspiciousDevicesAsync(string userId)
        {
            var devices = await _context.UserDevices
                .Where(d => d.UserId == userId)
                .ToListAsync();

            var suspicious = new List<UserDevice>();

            foreach (var device in devices)
            {
                // Check for suspicious patterns
                if (!device.IsTrusted && device.FirstSeen > DateTime.UtcNow.AddDays(-1))
                {
                    suspicious.Add(device);
                }

                // Multiple IPs for same device
                var sameDeviceCount = devices.Count(d => d.DeviceFingerprint == device.DeviceFingerprint);
                if (sameDeviceCount > 3)
                {
                    suspicious.Add(device);
                }
            }

            return suspicious.Distinct().ToList();
        }

        private string? GetBrowserName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return null;
            var ua = userAgent.ToLower();

            if (ua.Contains("edge")) return "Edge";
            if (ua.Contains("opr") || ua.Contains("opera")) return "Opera";
            if (ua.Contains("chrome")) return "Chrome";
            if (ua.Contains("safari")) return "Safari";
            if (ua.Contains("firefox")) return "Firefox";
            if (ua.Contains("msie") || ua.Contains("trident")) return "Internet Explorer";

            return "Unknown";
        }

        private string? GetOperatingSystem(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return null;
            var ua = userAgent.ToLower();

            if (ua.Contains("windows")) return "Windows";
            if (ua.Contains("macintosh") || ua.Contains("mac os")) return "macOS";
            if (ua.Contains("linux")) return "Linux";
            if (ua.Contains("android")) return "Android";
            if (ua.Contains("iphone") || ua.Contains("ipad")) return "iOS";

            return "Unknown";
        }
        private string GetDeviceType(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";
            var ua = userAgent.ToLower();

            if (ua.Contains("mobile") || ua.Contains("iphone") || ua.Contains("android")) return "Mobile";
            if (ua.Contains("ipad") || ua.Contains("tablet")) return "Tablet";
            if (ua.Contains("windows") || ua.Contains("macintosh") || ua.Contains("linux")) return "Desktop";

            return "Unknown";
        }


    }
}

