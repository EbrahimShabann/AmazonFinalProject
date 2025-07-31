using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.CreateUserViewModel
{
    public class CreateUserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string SelectedRole { get; set; }
        public DateTime birthdate { get; set; }
        public IFormFile imgFile { get; set; } // For file upload

        public List<string> Roles { get; set; } = new List<string> { "seller", "CustomerService" };

    }

}
