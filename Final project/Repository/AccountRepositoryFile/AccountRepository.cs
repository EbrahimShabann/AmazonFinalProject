using System.Runtime.Intrinsics.Arm;
using Final_project.Models;
using Final_project.ViewModel.AccountPageViewModels;

namespace Final_project.Repository.AccountRepositoryFile
{
    public class AccountRepository:IAccountRepository
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
            user.last_login = DateTime.UtcNow;
            db.SaveChanges();
        }

        private ApplicationUser GetUserById(string UserId)
        {
            return db.Users.FirstOrDefault(u => u.Id == UserId);
        }
        public bool UpdateUserLogs(ApplicationUser user, string Action)
        {
            if (user != null&&Action !=null)
            {
                var logs = new AccountLog()
                {
                    UserID =user.Id,
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
            var foundUser = db.Users.FirstOrDefault(u=>u.UserName==username && u.is_active==true);
            if (foundUser != null) return true;
            else return false;
                
   
        }
    }
}
