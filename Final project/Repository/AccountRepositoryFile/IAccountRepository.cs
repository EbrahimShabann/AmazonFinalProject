using Final_project.Models;
using Final_project.ViewModel.AccountPageViewModels;

namespace Final_project.Repository.AccountRepositoryFile
{
    public interface IAccountRepository
    {
        public bool UpdateUserLogs(ApplicationUser user, string Action);
        public Task<bool> SetProfileAndBirthday(ProfilePic_DateOfBirth data);
        public void UpdateLastLog(string UserId);
        public bool IsApprovedSeller(string username);
        public bool CheckProfilePic(string userName);

        #region Phone Number and Two-Factor Management
        /// <summary>
        /// Updates the phone number confirmation status for a user
        /// </summary>
        public Task<bool> UpdatePhoneConfirmationAsync(string userId, bool isConfirmed);

        /// <summary>
        /// Updates the two-factor authentication enabled status for a user
        /// </summary>
        public Task<bool> UpdateTwoFactorEnabledAsync(string userId, bool isEnabled);

        /// <summary>
        /// Updates both phone confirmation and two-factor status
        /// </summary>
        public Task<bool> UpdatePhoneAndTwoFactorAsync(string userId, bool phoneConfirmed, bool twoFactorEnabled);

        /// <summary>
        /// Gets the phone confirmation and two-factor status for a user
        /// </summary>
        public Task<(bool phoneConfirmed, bool twoFactorEnabled)> GetPhoneAndTwoFactorStatusAsync(string userId);

        /// <summary>
        /// Checks if a user needs phone verification
        /// </summary>
        public Task<bool> RequiresPhoneVerificationAsync(string userId);

        /// <summary>
        /// Gets users who have unconfirmed phone numbers for cleanup/reminder purposes
        /// </summary>
        public Task<List<ApplicationUser>> GetUsersWithUnconfirmedPhonesAsync(int daysOld = 7);

        /// <summary>
        /// Updates phone number and sets confirmation status
        /// </summary>
        public Task<bool> UpdatePhoneNumberAsync(string userId, string phoneNumber, bool isConfirmed = false);
        #endregion

        #region Device Management
        public Task<List<UserDevice>> GetUserDevicesAsync(string userId);
        public Task<UserDevice> GetDeviceAsync(int deviceId);
        public Task RemoveDeviceAsync(int deviceId);
        public Task<bool> IsDeviceTrustedAsync(string userId, string deviceFingerprint);
        public Task MarkAllDevicesAsUntrustedAsync(string userId);
        public Task CleanupOldDevicesAsync(int daysOld = 90);
        public Task<int> GetTrustedDeviceCountAsync(string userId);
        public Task<List<UserDevice>> GetRecentLoginAttemptsAsync(string userId, int hours = 24);
        #endregion
    }
}