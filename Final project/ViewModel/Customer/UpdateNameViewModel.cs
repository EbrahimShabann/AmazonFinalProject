using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel.Customer
{
    public class UpdateNameViewModel
    {
        [Required]
        [Display(Name = "New name")]
        public string FullName { get; set; }
    }
}