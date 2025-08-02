using Final_project.Repository;
using Final_project.ViewModel.NewFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        // Updated Search method with enhanced product data for chatbot compatibility
        public IActionResult Search(string query)
        {
            var products = unitOfWork.LandingPageReposotory.ProductSearch(query);
            return Json(products);
        }

        [HttpPost]
        public IActionResult CartCount(string UserName)
        {
            var cart = unitOfWork.LandingPageReposotory.GetCartCount(UserName);
            return Json(cart);
        }

        [HttpGet]
        public IActionResult Error()
        {
            throw new NotImplementedException();
        }
    }
}