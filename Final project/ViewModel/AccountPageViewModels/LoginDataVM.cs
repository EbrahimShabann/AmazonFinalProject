using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class LoginDataVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
