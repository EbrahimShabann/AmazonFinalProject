using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers.Seller
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
