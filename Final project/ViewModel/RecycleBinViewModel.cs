using Final_project.Models;

namespace Final_project.ViewModel
{
    public class RecycleBinViewModel
    {
        public List<category> DeletedCategories { get; set; }
        public List<product> DeletedProducts { get; set; }
        public List<ApplicationUser> DeletedSellers { get; set; }
        public List<ApplicationUser> DeletedCustomerService { get; set; }
        public List<ApplicationUser> Users { get; set; }

    }
}
