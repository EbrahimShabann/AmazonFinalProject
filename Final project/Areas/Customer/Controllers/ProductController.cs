using Final_project.Repository;
using Final_project.Services.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly UnitOfWork uof;

        public ProductController(UnitOfWork uof)
        {
            this.uof = uof;
        }
        public IActionResult Index(int page = 1, int size = 10)
        {
            var products = uof.ProductRepository.getProductsWithImagesAndRating().ToPagedResult(page,size);
       
            return View(products);
        }
    }
}
