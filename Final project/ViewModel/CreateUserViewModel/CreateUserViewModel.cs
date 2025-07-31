using System.ComponentModel.DataAnnotations;
using Final_project.CustomAttribute;

namespace Final_project.ViewModel.CreateUserViewModel
{
    public class CreateUserViewModel
    {
        [UniqueName(ErrorMessage = "Username already exists.")]
        [Required]
        public string UserName { get; set; }

        [Required]
        [UniqueEmail(ErrorMessage = "Email already exists.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string SelectedRole { get; set; }
        public DateTime birthdate { get; set; }
        public IFormFile? imgFile { get; set; } // For file upload

        public List<string> Roles { get; set; } = new List<string> { "seller", "Support" };

    }

}