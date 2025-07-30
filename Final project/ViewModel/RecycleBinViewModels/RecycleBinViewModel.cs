using Final_project.Models;

namespace Final_project.ViewModel.RecycleBinViewModels
{
    public class RecycleBinViewModel
    {
        public List<DeletedCategories> DeletedCategories { get; set; } = new List<DeletedCategories>();
        public List<DeletedProducts> DeletedProducts { get; set; } = new List<DeletedProducts>();
        public List<DeletedUsers> DeletedCustomerService { get; set; } = new List<DeletedUsers>();
        public List<DeletedUsers> DeletedSellers { get; set; } = new List<DeletedUsers>();
        public List<ApplicationUser> Users { get; set; }

    }
}