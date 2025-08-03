using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class EnableTwoFactorVM
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; }
    }
}
