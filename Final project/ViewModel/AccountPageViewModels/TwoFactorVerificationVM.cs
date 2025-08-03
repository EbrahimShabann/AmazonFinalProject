using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class TwoFactorVerificationVM
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        public string UserId { get; set; }
        public string Purpose { get; set; }
        public int? DeviceId { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; } = "/Swithc/Index";
    }
}
