using System.Runtime.Intrinsics.Arm;
using Final_project.Models;
using Final_project.ViewModel.AccountPageViewModels;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.AccountRepositoryFile
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AmazonDBContext db;

        public AccountRepository(AmazonDBContext db)
        {
            this.db = db;
        }

        public async Task<bool> SetProfileAndBirthday(ProfilePic_DateOfBirth data)
        {
            if (data == null)
                return false;

            try
            {
                string uniqueFileName = null;

                if (data.ImageFile != null && data.ImageFile.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(data.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                        return false;

                    // Ensure directory exists
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Save file
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + data.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await data.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Update user
                var user = GetUserById(data.UserID);
                if (user == null)
                    return false;

                user.profile_picture_url = uniqueFileName;
                user.date_of_birth = data.Birthday;
                user.PhoneNumber = data.PhoneNumber;

                // If phone number is provided, set it as unconfirmed initially
                if (!string.IsNullOrEmpty(data.PhoneNumber))
                {
                    user.PhoneNumberConfirmed = "false";
                }

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public void UpdateLastLog(string UserId)
        {
            var user = GetUserById(UserId);
            if (user != null)
            {
                user.last_login = DateTime.UtcNow;
                db.SaveChanges();
            }
        }

        private ApplicationUser GetUserById(string UserId)
        {
            return db.Users.FirstOrDefault(u => u.Id == UserId);
        }

        public bool UpdateUserLogs(ApplicationUser user, string Action)
        {
            if (user != null && Action != null)
            {
                var logs = new AccountLog()
                {
                    UserID = user.Id,
                    ActionType = Action,
                    AdditionalInfo = "Nothing",
                };
                db.AccountLog.Add(logs);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool IsApprovedSeller(string username)
        {
            var foundUser = db.Users.FirstOrDefault(u => u.UserName == username && u.is_active == true);
            if (foundUser != null) return true;
            else return false;
        }

        public bool CheckProfilePic(string userName)
        {
            var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
            var profilePictureUrl = user?.profile_picture_url;
            if (profilePictureUrl != null && profilePictureUrl.Length > 0) return true;
            else return false;
        }

        #region Phone Number and Two-Factor Management

        /// <summary>
        /// Updates the phone number confirmation status for a user
        /// </summary>
        public async Task<bool> UpdatePhoneConfirmationAsync(string userId, bool isConfirmed)
        {
            try
            {
                var user = await db.Users.FindAsync(userId);
                if (user != null)
                {
                    user.PhoneNumberConfirmed = isConfirmed ? "true" : "false";
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        /// <summary>
        /// Updates the two-factor authentication enabled status for a user
        /// </summary>
        public async Task<bool> UpdateTwoFactorEnabledAsync(string userId, bool isEnabled)
        {
            try
            {
                var user = await db.Users.FindAsync(userId);
                if (user != null)
                {
                    user.TwoFactorEnabled = isEnabled;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        /// <summary>
        /// Updates both phone confirmation and two-factor status
        /// </summary>
        public async Task<bool> UpdatePhoneAndTwoFactorAsync(string userId, bool phoneConfirmed, bool twoFactorEnabled)
        {
            try
            {
                var user = await db.Users.FindAsync(userId);
                if (user != null)
                {
                    user.PhoneNumberConfirmed = phoneConfirmed ? "true" : "false";
                    user.TwoFactorEnabled = twoFactorEnabled;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        /// <summary>
        /// Gets the phone confirmation and two-factor status for a user
        /// </summary>
        public async Task<(bool phoneConfirmed, bool twoFactorEnabled)> GetPhoneAndTwoFactorStatusAsync(string userId)
        {
            try
            {
                var user = await db.Users.FindAsync(userId);
                if (user != null)
                {
                    return (user.PhoneNumberConfirmed == "true", user.TwoFactorEnabled);
                }
                return (false, false);
            }
            catch
            {
                return (false, false);
            }
        }

        /// <summary>
        /// Checks if a user needs phone verification
        /// </summary>
        public async Task<bool> RequiresPhoneVerificationAsync(string userId)
        {
            try
            {
                var user = await db.Users.FindAsync(userId);
                return user != null &&
                       !string.IsNullOrEmpty(user.PhoneNumber) &&
                       user.PhoneNumberConfirmed != "true";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets users who have unconfirmed phone numbers for cleanup/reminder purposes
        /// </summary>
        public async Task<List<ApplicationUser>> GetUsersWithUnconfirmedPhonesAsync(int daysOld = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                return await db.Users
                    .Where(u => !string.IsNullOrEmpty(u.PhoneNumber) &&
                               u.PhoneNumberConfirmed != "true" &&
                               u.created_at <= cutoffDate)
                    .ToListAsync();
            }
            catch
            {
                return new List<ApplicationUser>();
            }
        }

        /// <summary>
        /// Updates phone number and sets confirmation status
        /// </summary>
        public async Task<bool> UpdatePhoneNumberAsync(string userId, string phoneNumber, bool isConfirmed = false)
        {
            try
            {
                var user = await db.Users.FindAsync(userId);
                if (user != null)
                {
                    user.PhoneNumber = phoneNumber;
                    user.PhoneNumberConfirmed = isConfirmed ? "true" : "false";
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        #endregion

        #region Device Management (Existing methods)

        public async Task<List<UserDevice>> GetUserDevicesAsync(string userId)
        {
            return await db.UserDevices
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.LastSeen)
                .ToListAsync();
        }

        public async Task<UserDevice> GetDeviceAsync(int deviceId)
        {
            return await db.UserDevices.FindAsync(deviceId);
        }

        public async Task RemoveDeviceAsync(int deviceId)
        {
            var device = await db.UserDevices.FindAsync(deviceId);
            if (device != null)
            {
                db.UserDevices.Remove(device);
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsDeviceTrustedAsync(string userId, string deviceFingerprint)
        {
            return await db.UserDevices
                .AnyAsync(d => d.UserId == userId &&
                              d.DeviceFingerprint == deviceFingerprint &&
                              d.IsTrusted);
        }

        public async Task MarkAllDevicesAsUntrustedAsync(string userId)
        {
            var devices = await db.UserDevices
                .Where(d => d.UserId == userId)
                .ToListAsync();

            foreach (var device in devices)
            {
                device.IsTrusted = false;
            }

            await db.SaveChangesAsync();
        }

        public async Task CleanupOldDevicesAsync(int daysOld = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            var oldDevices = await db.UserDevices
                .Where(d => d.LastSeen < cutoffDate && !d.IsTrusted)
                .ToListAsync();

            db.UserDevices.RemoveRange(oldDevices);
            await db.SaveChangesAsync();
        }

        public async Task<int> GetTrustedDeviceCountAsync(string userId)
        {
            return await db.UserDevices
                .CountAsync(d => d.UserId == userId && d.IsTrusted);
        }

        public async Task<List<UserDevice>> GetRecentLoginAttemptsAsync(string userId, int hours = 24)
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-hours);
            return await db.UserDevices
                .Where(d => d.UserId == userId && d.LastSeen >= cutoffTime)
                .OrderByDescending(d => d.LastSeen)
                .ToListAsync();
        }

        #endregion

        public string GetUserPhoneNumber(string UserId)
        {
            return db.Users.FirstOrDefault(u => u.Id == UserId).PhoneNumber;
        }

    }
}