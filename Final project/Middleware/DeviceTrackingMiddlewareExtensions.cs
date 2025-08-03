namespace Final_project.Middleware
{
    public static class DeviceTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder UseDeviceTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DeviceTrackingMiddleware>();
        }
    }
}
