using Final_project.ViewModel.LandingPageViewModels;
using Final_project.ViewModel.NewFolder;

namespace Final_project.Repository.NewFolder
{
    public interface ILandingPageRepository
    {
        public List<LandingPageProductDiscount> GetNewDiscounts(int Number=10);
        public List<LandingPageProducts> GetBestSellers(int Number = 10);
        public List<LandingPageProducts> GetNewArrivals(int Number = 10);
        public List<ProductSearchViewModel> ProductSearch(string searchTerm, int pageNumber = 1, int pageSize = 10);
        public string GetProductImageUrl(string productId);
        public int GetProductRating(string productId);
        public int GetProductRatingCount(string productId);


        //Category



    }
}
