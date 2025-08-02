using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
