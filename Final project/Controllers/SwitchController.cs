using Final_project.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class SwitchController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public SwitchController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            if (User.IsInRole("admin")) return RedirectToAction("Index", "AdminDashboard");
            else if (User.IsInRole("seller"))
            {
                if(unitOfWork.AccountRepository.IsApprovedSeller(User.Identity.Name))
                return RedirectToAction("SellerDashboard", "seller");
                else
                    return RedirectToAction("WatingforAdminApproval", "Account");


            }
            else if (User.IsInRole("customerService")) return RedirectToAction("Index", "CustomerService");

            else return RedirectToAction("Index", "Profile");

        }
    }
}
