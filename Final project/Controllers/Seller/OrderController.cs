using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers.Seller
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
