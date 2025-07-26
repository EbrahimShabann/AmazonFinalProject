using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.Customer
{
    public class UpdatePhoneViewModel
    {
        [Required, Phone]
        [Display(Name = "New Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
