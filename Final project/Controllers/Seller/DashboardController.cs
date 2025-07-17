using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers.Seller
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
