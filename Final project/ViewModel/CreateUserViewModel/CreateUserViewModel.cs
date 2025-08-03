using System.ComponentModel.DataAnnotations;
using Final_project.CustomAttribute;
using Microsoft.AspNetCore.Http;

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

        [Phone]
        [Display(Name = "Phone Number")]
        [EgyptianPhone]
        public string PhoneNumber { get; set; } // ✅ Add this line

        public DateTime birthdate { get; set; }

        public IFormFile imgFile { get; set; }

        public List<string> Roles { get; set; } = new List<string> { "seller", "customerService" };
    }
}
