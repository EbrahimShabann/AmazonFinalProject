using Final_project.Models;
using Final_project.Services.SmsService;
using Final_project.Services.DeviceService;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Services.TwoFactorService
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly AmazonDBContext _context;
        private readonly ISmsService _smsService;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<TwoFactorService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TwoFactorService(AmazonDBContext context, ISmsService smsService,
            IDeviceService deviceService, ILogger<TwoFactorService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _smsService = smsService;
            _deviceService = deviceService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateCodeAsync(string userId, string purpose)
        {
            // Generate 6-digit code
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            // Get device fingerprint and IP from current HTTP context
            string deviceFingerprint = "Unknown";
            string ipAddress = "Unknown";

            if (_httpContextAccessor.HttpContext != null)
            {
                // Use the existing synchronous method from EnhancedDeviceService
                deviceFingerprint = _deviceService.GenerateDeviceFingerprint(_httpContextAccessor.HttpContext);

                // Get IP address using the same logic as in your service
                ipAddress = GetClientIpAddress(_httpContextAccessor.HttpContext);
            }

            var twoFactorCode = new TwoFactorCode
            {
                UserId = userId,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                Purpose = purpose,
                DeviceFingerprint = deviceFingerprint, // Now providing the required value
                IpAddress = ipAddress
            };

            _context.TwoFactorCodes.Add(twoFactorCode);
            await _context.SaveChangesAsync();
            return code;
        }

        public async Task<bool> ValidateCodeAsync(string userId, string code, string purpose)
        {
            var twoFactorCode = await _context.TwoFactorCodes.FirstOrDefaultAsync(c => c.UserId == userId &&
                                        c.Code == code &&
                                        c.Purpose == purpose &&
                                        !c.IsUsed &&
                                        c.ExpiresAt > DateTime.UtcNow);

            if (twoFactorCode != null)
            {
                twoFactorCode.IsUsed = true;
                twoFactorCode.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> SendLoginVerificationAsync(string userId, string phoneNumber)
        {
            var code = await GenerateCodeAsync(userId, "Login");
            return await _smsService.SendVerificationCodeAsync(phoneNumber, code);
        }

        public async Task CleanExpiredCodesAsync()
        {
            var expiredCodes = await _context.TwoFactorCodes
                .Where(c => c.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
            _context.TwoFactorCodes.RemoveRange(expiredCodes);
            await _context.SaveChangesAsync();
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
    }
}