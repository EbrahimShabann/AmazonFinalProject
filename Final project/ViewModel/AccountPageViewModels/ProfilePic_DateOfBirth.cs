using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.ViewModel.AccountPageViewModels
{
    public class ProfilePic_DateOfBirth
    {
        public string UserID { get; set; }
        public DateTime? Birthday { get; set; }
        public String PhoneNumber { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
