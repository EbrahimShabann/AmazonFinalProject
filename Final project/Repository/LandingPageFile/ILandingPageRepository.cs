using Final_project.ViewModel.NewFolder;

namespace Final_project.Repository.NewFolder
{
    public interface ILandingPageRepository
    {
        public List<LandingPageProductDiscount> GetNewDiscounts(int Number=10);
        public List<LandingPageProducts> GetBestSellers(int Number = 10);
        public List<LandingPageProducts> GetNewArrivals(int Number = 10);


    }
}
