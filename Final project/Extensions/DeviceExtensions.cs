using Final_project.Models;

namespace Final_project.Extensions
{
    public static class DeviceExtensions
    {
        public static string GetDeviceIcon(this UserDevice device)
        {
            if (device.DeviceName.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
                return "mobile-alt";
            if (device.DeviceName.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
                return "tablet-alt";
            return "desktop";
        }

        public static string GetDeviceTypeClass(this UserDevice device)
        {
            if (device.DeviceName.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
                return "text-info";
            if (device.DeviceName.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
                return "text-warning";
            return "text-primary";
        }

        public static bool IsRecentLogin(this UserDevice device, int hoursThreshold = 1)
        {
            return device.LastSeen >= DateTime.UtcNow.AddHours(-hoursThreshold);
        }

        public static string GetLocationInfo(this UserDevice device)
        {
            // You can enhance this with a GeoIP service
            return $"IP: {device.IpAddress}";
        }
    }
}
