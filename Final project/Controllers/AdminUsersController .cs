using Final_project.Models;
using Final_project.Repository;
using Final_project.ViewModel.AdminUsers;
using Final_project.ViewModel.CreateUserViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UnitOfWork _unitOfWork;

        public AdminUsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        // GET: AdminUsers/AddUser
        [HttpGet]
        public IActionResult AddUser()
        {
            var model = new CreateUserViewModel();
            return View(model);
        }

        // POST: AdminUsers/AddUser
        [HttpPost]
        public async Task<IActionResult> AddUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string profilePictureFileName = null;

            // Handle profile picture upload
            if (model.imgFile != null && model.imgFile.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(model.imgFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("imgFile", "Please upload a valid image file (jpg, jpeg, png, gif).");
                    return View(model);
                }

                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
                Directory.CreateDirectory(uploads);

                // Generate unique filename to avoid conflicts
                profilePictureFileName = Guid.NewGuid().ToString() + "_" + model.imgFile.FileName;
                var filePath = Path.Combine(uploads, profilePictureFileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await model.imgFile.CopyToAsync(stream);
            }

            // Create user with all verifications bypassed (admin-created users are pre-verified)
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                date_of_birth = model.birthdate,
                profile_picture_url = profilePictureFileName,
                PhoneNumber = model.PhoneNumber,

                // Skip all verification requirements for admin-created users
                EmailConfirmed = true,                    // Email is pre-confirmed
                PhoneNumberConfirmed = "true",           // Phone is pre-confirmed (if provided)
                TwoFactorEnabled = false,                // 2FA disabled by default (admin can enable later if needed)

                // Set timestamps
                created_at = DateTime.UtcNow,
                last_login = DateTime.UtcNow,

                // Admin-created users are automatically active
                is_active = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Verify the role exists
                if (!await _roleManager.RoleExistsAsync(model.SelectedRole))
                {
                    ModelState.AddModelError("", "Selected role does not exist.");
                    return View(model);
                }

                // Add user to selected role
                await _userManager.AddToRoleAsync(user, model.SelectedRole);

                // Log the admin action
                _unitOfWork.AccountRepository.UpdateUserLogs(user, $"User created by admin with role: {model.SelectedRole}");

                TempData["Success"] = $"User '{model.UserName}' added successfully with role '{model.SelectedRole}'! All verifications have been bypassed.";
                return RedirectToAction("Index", "AdminDashboard");
            }

            // Handle creation errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // Optional: Method to enable 2FA for specific admin-created users if needed later
        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorForUser(string userId, string phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.PhoneNumber = phoneNumber;
                user.PhoneNumberConfirmed = "true"; // Admin-verified phone number
                user.TwoFactorEnabled = true;

                await _userManager.UpdateAsync(user);
                _unitOfWork.AccountRepository.UpdateUserLogs(user, "Two-factor authentication enabled by admin");

                TempData["Success"] = "Two-factor authentication enabled for user.";
            }
            else
            {
                TempData["Error"] = "User not found.";
            }

            return RedirectToAction("Index", "AdminDashboard");
        }

        // Optional: Method to manage admin-created user settings
        [HttpGet]
        public async Task<IActionResult> ManageUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new ManageUserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed == "true",
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsActive = user.is_active,
                CurrentRoles = userRoles.ToList(),
                ProfilePictureUrl = user.profile_picture_url
            };

            return View(model);
        }
    }

}