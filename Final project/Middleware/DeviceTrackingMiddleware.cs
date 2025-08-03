using Final_project.Services.DeviceService;
using System.Security.Claims;

namespace Final_project.Middleware
{
    public class DeviceTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DeviceTrackingMiddleware> _logger;

        public DeviceTrackingMiddleware(RequestDelegate next, ILogger<DeviceTrackingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only track authenticated users
            if (context.User.Identity.IsAuthenticated)
            {
                try
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var deviceService = context.RequestServices.GetService<IDeviceService>();
                        if (deviceService != null)
                        {
                            // Update device information on each authenticated request
                            await deviceService.GetOrCreateDeviceAsync(userId, context);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error tracking device information");
                }
            }

            await _next(context);
        }
    }
}
