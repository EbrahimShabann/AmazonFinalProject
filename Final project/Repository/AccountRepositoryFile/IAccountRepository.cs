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
    }
}
