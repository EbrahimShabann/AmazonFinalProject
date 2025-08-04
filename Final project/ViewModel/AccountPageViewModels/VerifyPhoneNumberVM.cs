using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class VerifyPhoneNumberVM
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Masked Phone Number")]
        public string MaskedPhoneNumber { get; set; }

        public string Purpose { get; set; } // "Login" or "Registration"

        public bool RememberMe { get; set; }
    }
}