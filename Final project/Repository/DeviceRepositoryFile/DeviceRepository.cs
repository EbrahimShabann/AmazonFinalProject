using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.DeviceRepositoryFile
{
    public class DeviceRepository:IDeviceRepository
    {
        private readonly AmazonDBContext context;

        public DeviceRepository(AmazonDBContext context)
        {
            this.context = context;
        }

        public async Task<UserDevice> CreateDeviceAsync(UserDevice device)
        {
            context.UserDevices.Add(device);
            await context.SaveChangesAsync();
            return device;
        }

        public async Task<UserDevice> UpdateDeviceAsync(UserDevice device)
        {
            context.UserDevices.Update(device);
            await context.SaveChangesAsync();
            return device;
        }

        public async Task<UserDevice> GetDeviceByFingerprintAsync(string userId, string fingerprint)
        {
            return await context.UserDevices.FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceFingerprint == fingerprint);

        }

        public async Task<List<UserDevice>> GetUserDevicesAsync(string userId)
        {
            return await context.UserDevices
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.LastSeen)
                .ToListAsync();
        }

        public async Task<bool> RemoveDeviceAsync(int deviceId, string userId)
        {
            var device = await context.UserDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId);

            if (device != null)
            {
                context.UserDevices.Remove(device);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task MarkDeviceAsTrustedAsync(int deviceId, string userId)
        {
            var device = await context.UserDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId);

            if (device != null)
            {
                device.IsTrusted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllDevicesAsync(string userId)
        {
            var devices = await context.UserDevices
                .Where(d => d.UserId == userId)
                .ToListAsync();

            foreach (var device in devices)
            {
                device.IsTrusted = false;
            }

            await context.SaveChangesAsync();
        }
    }
}
