namespace Final_project.Services.TwoFactorService
{
    public interface ITwoFactorService
    {
        public Task<string> GenerateCodeAsync(string userId, string purpose);
        public Task<bool> ValidateCodeAsync(string userId, string code, string purpose);
        public Task<bool> SendLoginVerificationAsync(string userId, string phoneNumber);
        public Task CleanExpiredCodesAsync();
    }
}
