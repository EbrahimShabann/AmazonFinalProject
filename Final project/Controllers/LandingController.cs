using Final_project.Repository;
using Final_project.ViewModel.NewFolder;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class LandingController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public LandingController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            unitOfWork.CategoryRepository.GetCategoryWithItsChildern();
            var DataForPage = new LandingPageViewModel()
            {
                BestSellers = unitOfWork.LandingPageReposotory.GetBestSellers(),
                NewArrivals = unitOfWork.LandingPageReposotory.GetNewArrivals(),
                DiscountProducts = unitOfWork.LandingPageReposotory.GetNewDiscounts()
            };

            return View(DataForPage);
        }
    
        public IActionResult Search(string query)
        {
            return Json(unitOfWork.LandingPageReposotory.ProductSearch(query));
        }
    
    }
}
