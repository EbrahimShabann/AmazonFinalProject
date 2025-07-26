using Final_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class AdminCustomerServiceController : Controller
    {
        private readonly AmazonDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminCustomerServiceController(AmazonDBContext context, UserManager<ApplicationUser> _userManager)
        {
            _context = context;
            this._userManager = _userManager;
        }


        public async Task<IActionResult> pendingCustomerService()
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support"));
            ViewBag.CountPendingCustomerService = CustomerService.Where(u => !u.is_deleted & !u.is_active).Count();
            ViewBag.CountAcceptedCustomerService = CustomerService.Where(u => !u.is_deleted & u.is_active).Count();
            ViewBag.CountRegectedCustomerService = CustomerService.Where(u => u.is_deleted & !u.is_active).Count();
            ViewBag.PendeingCustomerService = CustomerService.Where(u => !u.is_deleted & !u.is_active).OrderByDescending(u => u.created_at).ToList();

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
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> RejectCustomerService(string id)
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support")).FirstOrDefault(u => u.Id == id);
            if (CustomerService == null) return Json(new { success = false });

            CustomerService.is_active = false;
            CustomerService.is_deleted = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }
        public async Task<JsonResult> inactiveCustomerService(string id)
        {
            var CustomerService = (await _userManager.GetUsersInRoleAsync("Support")).FirstOrDefault(u => u.Id == id);
            if (CustomerService == null) return Json(new { success = false });

            CustomerService.is_active = false;
            CustomerService.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
