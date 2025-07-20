using Final_project.ViewModel.LandingPageViewModels;
using Final_project.ViewModel.NewFolder;

namespace Final_project.Repository.CategoryFile
{
    public interface ICategoryRepository
    {
        public List<CategoryViewModel> GetCategoryWithItsChildern();
        public int totalProduct();


    }
}
