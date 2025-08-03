using Final_project.Models;
using Final_project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Final_project.Controllers
{
    [Authorize(Roles = "admin,superadmin")]

    public class AdminSellersController : Controller
    {

        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminSellersController(UnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager)
        {
            this.unitOfWork = unitOfWork;
            this._userManager = _userManager;

        }

        public async Task<IActionResult> pendingseller()
        {
            var seller = (await _userManager.GetUsersInRoleAsync("Seller"));
            ViewBag.CountPendingsellers = seller.Where(u => !u.is_deleted & !u.is_active).Count();
            ViewBag.CountAcceptedsellers = seller.Where(u => !u.is_deleted & u.is_active).Count();
            ViewBag.CountRegectedsellers = seller.Where(u => u.is_deleted & !u.is_active).Count();
            ViewBag.Pendeingsellers = seller.Where(u => !u.is_deleted & !u.is_active).OrderByDescending(u => u.created_at).ToList();

            return View("pendingSeller");
        }
        public async Task<IActionResult> Allsellers()
        {
            var seller = (await _userManager.GetUsersInRoleAsync("Seller"));
            ViewBag.CountAllsellers = seller.Where(u => !u.is_deleted).Count();
            ViewBag.CountActivesellers = seller.Where(u => !u.is_deleted & u.is_active).Count();
            ViewBag.CountInactivesellers = seller.Where(u => !u.is_deleted & !u.is_active).Count();

            return View(seller.Where(u => !u.is_deleted).ToList());
        }

        [HttpPost]
        public async Task<JsonResult> Approveseller(string id)
        {
            var seller = (await _userManager.GetUsersInRoleAsync("Seller")).FirstOrDefault(u => u.Id == id);
            if (seller == null) return Json(new { success = false });

            seller.is_active = true;
            seller.is_deleted = false;
            unitOfWork.save();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<JsonResult> Rejectseller(string id)
        {
            var seller = (await _userManager.GetUsersInRoleAsync("Seller")).FirstOrDefault(u => u.Id == id);
            if (seller == null) return Json(new { success = false });

            seller.is_active = false;
            seller.is_deleted = true;
            seller.deleted_at = DateTime.UtcNow;
            unitOfWork.save();

            return Json(new { success = true });
        }
        public async Task<JsonResult> inactiveseller(string id)
        {
            var seller = (await _userManager.GetUsersInRoleAsync("Seller")).FirstOrDefault(u => u.Id == id);
            if (seller == null) return Json(new { success = false });

            seller.is_active = false;
            seller.is_deleted = false;
            unitOfWork.save();

            return Json(new { success = true });
        }
    }

}