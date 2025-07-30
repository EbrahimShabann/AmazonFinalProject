using Final_project.Models;
using Final_project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminCustomerServiceController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminCustomerServiceController(UnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager)
        {
            this.unitOfWork = unitOfWork;
            this._userManager = _userManager;
        }


        public async Task<IActionResult> pendingCustomerService()
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support"));
            ViewBag.CountPendingCustomerService = CustomerService.Where(u => !u.is_deleted & !u.is_active).Count();
            ViewBag.CountAcceptedCustomerService = CustomerService.Where(u => !u.is_deleted & u.is_active).Count();
            ViewBag.CountRegectedCustomerService = CustomerService.Where(u => u.is_deleted & !u.is_active).Count();
            ViewBag.PendingCustomerService = CustomerService.Where(u => !u.is_deleted & !u.is_active).OrderByDescending(u => u.created_at).ToList();

            return View();
        }
        public async Task<IActionResult> AllCustomerService()
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support"));
            ViewBag.CountAllCustomerService = CustomerService.Where(u => !u.is_deleted).Count();
            ViewBag.CountActiveCustomerService = CustomerService.Where(u => !u.is_deleted & u.is_active).Count();
            ViewBag.CountInactiveCustomerService = CustomerService.Where(u => !u.is_deleted & !u.is_active).Count();

            return View(CustomerService.Where(u => !u.is_deleted).ToList());
        }

        [HttpPost]
        public async Task<JsonResult> ApproveCustomerService(string id)
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support")).FirstOrDefault(u => u.Id == id);
            if (CustomerService == null) return Json(new { success = false });

            CustomerService.is_active = true;
            CustomerService.is_deleted = false;
            unitOfWork.save();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<JsonResult> RejectCustomerService(string id)
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support")).FirstOrDefault(u => u.Id == id);
            if (CustomerService == null) return Json(new { success = false });

            CustomerService.is_active = false;
            CustomerService.is_deleted = true;
            CustomerService.deleted_at = DateTime.UtcNow;
            unitOfWork.save();

            return Json(new { success = true });
        }
        public async Task<JsonResult> inactiveCustomerService(string id)
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support")).FirstOrDefault(u => u.Id == id);
            if (CustomerService == null) return Json(new { success = false });

            CustomerService.is_active = false;
            CustomerService.is_deleted = false;
            
            unitOfWork.save();

            return Json(new { success = true });
        }
    }
}