using Final_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    public class AdminSellersController : Controller
    {
       
        private readonly AmazonDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminSellersController(AmazonDBContext context, UserManager<ApplicationUser> _userManager)
        {
            _context = context;
            this._userManager = _userManager;
        }


        public async Task<IActionResult> pendingSellers()
        {
            var sellers = (await _userManager.GetUsersInRoleAsync("Seller")); 
            ViewBag.CountPendingSellers = sellers.Where( u=>   !u.is_deleted & !u.is_active).Count();
            ViewBag.CountAcceptedSellers = sellers.Where(u => !u.is_deleted & u.is_active).Count();
            ViewBag.CountRegectedSellers = sellers.Where(u => u.is_deleted & !u.is_active).Count();
            ViewBag.PendeingSellers = sellers.Where(u => !u.is_deleted & !u.is_active).OrderByDescending(u => u.created_at).ToList();
            ViewBag.ActivePage = "Products";

            return View();
        }
        public async Task<IActionResult> AllSellers()
        {
            var sellers = (await _userManager.GetUsersInRoleAsync("Seller"));
            ViewBag.CountAllSellers = sellers.Where(u=>!u.is_deleted).Count();
            ViewBag.CountActiveSellers = sellers.Where(u => !u.is_deleted & u.is_active).Count();
            ViewBag.CountInactiveSellers = sellers.Where(u => !u.is_deleted & !u.is_active).Count();

            return View(sellers.Where(u=>!u.is_deleted).ToList());
        }


        [HttpPost]
        public async Task<JsonResult> ApproveSeller(string id)
        {
            var sellers = (await _userManager.GetUsersInRoleAsync("Seller")).FirstOrDefault(u => u.Id == id);
            if (sellers == null) return Json(new { success = false });

            sellers.is_active = true;
            sellers.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> RejectSeller(string id)
        {
            var sellers = (await _userManager.GetUsersInRoleAsync("Seller")).FirstOrDefault(u=>u.Id==id);
            if (sellers == null) return Json(new { success = false });

            sellers.is_active = false;
            sellers.is_deleted = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }
        public async Task<JsonResult> inactiveSeller(string id)
        {
            var sellers = (await _userManager.GetUsersInRoleAsync("Seller")).FirstOrDefault(u => u.Id == id);
            if (sellers == null) return Json(new { success = false });

            sellers.is_active = false;
            sellers.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }

}
