using Final_project.ViewModel.LandingPageViewModels;

namespace Final_project.Repository.CategoryFile
{
    public interface ICategoryRepository
    {
        public List<CategoryViewModel> GetCategoryWithItsChildern(); 
    }
}
