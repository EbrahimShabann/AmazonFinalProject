using Final_project.Models;
using Final_project.Services.TwoFactorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TwoFactorAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AmazonDBContext _context;
        private readonly ITwoFactorService _twoFactorService;

        public TwoFactorAdminController(UserManager<ApplicationUser> userManager, AmazonDBContext context, ITwoFactorService twoFactorService)
        {
            _userManager = userManager;
            _context = context;
            _twoFactorService = twoFactorService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.UserDevices)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.TwoFactorEnabled,
                    u.PhoneNumber,
                    DeviceCount = u.UserDevices.Count,
                    TrustedDevices = u.UserDevices.Count(d => d.IsTrusted),
                    LastLogin = u.UserDevices.Max(d => d.LastSeen)
                })
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DisableTwoFactorForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.TwoFactorEnabled = false;
                await _userManager.UpdateAsync(user);

                // Remove all trusted devices to force re-verification
                var devices = await _context.UserDevices.Where(d => d.UserId == userId).ToListAsync();
                foreach (var device in devices)
                {
                    device.IsTrusted = false;
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Two-factor authentication disabled for {user.UserName}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RevokeAllDevices(string userId)
        {
            var devices = await _context.UserDevices.Where(d => d.UserId == userId).ToListAsync();
            foreach (var device in devices)
            {
                device.IsTrusted = false;
            }
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            TempData["Success"] = $"All devices revoked for {user?.UserName}";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UserDevices(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var devices = await _context.UserDevices
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.LastSeen)
                .ToListAsync();

            ViewBag.User = user;
            return View(devices);
        }
    }
}
