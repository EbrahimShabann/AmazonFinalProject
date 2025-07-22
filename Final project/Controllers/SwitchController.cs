using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class SwitchController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index","Landing");
        }
    }
}
