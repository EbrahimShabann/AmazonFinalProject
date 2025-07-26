using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.Customer
{
    public class UpdateEmailViewModel
    {
        [Required, EmailAddress]
        public string NewEmail { get; set; }
    }
}
