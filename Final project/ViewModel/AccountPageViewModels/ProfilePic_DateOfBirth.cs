using System.ComponentModel.DataAnnotations.Schema;
using Final_project.CustomAttribute;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class ProfilePic_DateOfBirth
    {
        public string UserID { get; set; }
        public DateTime? Birthday { get; set; }
        [EgyptianPhone]
        public String PhoneNumber { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
