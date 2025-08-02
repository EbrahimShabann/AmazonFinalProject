using Final_project.CustomAttribute;
using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class RegisterDataVM
    {
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(50, ErrorMessage = "User name must be at most 50 characters.")]
        [UniqueName(ErrorMessage = "This user name is already taken.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [UniqueEmail(ErrorMessage = "This email address is already signed in.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public bool TermsAndPrivacy { get; set; }
        public bool SubscribeForNewsletter { get; set; }

    }
}
