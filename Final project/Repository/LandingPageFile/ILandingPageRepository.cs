using Final_project.ViewModel.LandingPageViewModels;
using Final_project.ViewModel.NewFolder;

namespace Final_project.Repository.NewFolder
{
    public interface ILandingPageRepository
    {
        public List<LandingPageProductDiscount> GetNewDiscounts(int take = 10, int skip = 0);
        public List<LandingPageProducts> GetBestSellers(int take = 10, int skip = 0);
        public List<LandingPageProducts> GetNewArrivals(int take = 10, int skip = 0);
        public List<ProductSearchViewModel> ProductSearch(string searchTerm, int pageNumber = 1, int pageSize = 10);
        public string GetProductImageUrl(string productId);
        public int GetProductRating(string productId);
        public int GetProductRatingCount(string productId);

        //cart
        public int GetCartCount(string Username);

        //Category
        public List<LandingPageProducts> GetFilteredProducts(ProductFilterParameters filterParams);
        public int GetFilteredProductsCount(ProductFilterParameters filterParams);


    }
}