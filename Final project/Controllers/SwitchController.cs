using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class SwitchController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("admin")) return RedirectToAction("Index", "AdminDashboard");
            else if (User.IsInRole("seller")) return RedirectToAction("SellerDashboard", "seller");
            else if (User.IsInRole("seller")) return RedirectToAction("Index", "AdminDashboard");

            else return RedirectToAction("Index", "Landing");

        }
    }
}
