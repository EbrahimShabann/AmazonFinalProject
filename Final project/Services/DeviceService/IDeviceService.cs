using Final_project.Models;

namespace Final_project.Services.DeviceService
{
    public interface IDeviceService
    {

        string GenerateDeviceFingerprint(HttpContext context);
        Task<UserDevice> GetOrCreateDeviceAsync(string userId, HttpContext context);
        Task<bool> IsNewDeviceAsync(string userId, HttpContext context);
        Task MarkDeviceAsTrustedAsync(int deviceId);
        Task<List<UserDevice>> GetSuspiciousDevicesAsync(string userId);


    }
}
