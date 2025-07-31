using System.Security.Claims;
using System.Threading.Tasks;
using Final_project.Models;
using Final_project.Repository;
using Final_project.ViewModel.AccountPageViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Final_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginDataVM loginData)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(loginData.Email);
                if (user != null)
                {
                    bool found = await userManager.CheckPasswordAsync(user, loginData.Password);
                    if (found)
                    {
                        await signInManager.SignInAsync(user, loginData.RememberMe);
                        var data = unitOfWork.AccountRepository.UpdateUserLogs(user, "Login");
                        unitOfWork.AccountRepository.UpdateLastLog(user.Id);
                        return RedirectToAction("Index", "Switch");
                    }
                }
                ModelState.AddModelError("", "Invalid Account");
            }
            return View(loginData);
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDataVM registerData)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = registerData.UserName,
                    Email = registerData.Email,
                    PasswordHash = registerData.Password,

                };
                IdentityResult created = await userManager.CreateAsync(user, registerData.Password);
                if (created.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "customer");
                    unitOfWork.AccountRepository.UpdateUserLogs(user, "Register");
                    await signInManager.SignInAsync(user, false);

                    return RedirectToAction("SetProfilePic", "Account", new { userId = user.Id });

                }
                foreach (var item in created.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

            }
            return View("Register", registerData);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                redirectUrl);

            // Force fresh login every time
            properties.Items["prompt"] = "select_account";
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user != null)
                {
                    unitOfWork.AccountRepository.UpdateUserLogs(user, "Google Account Login");
                    await userManager.UpdateAsync(user);

                }
                return LocalRedirect(returnUrl ?? "/");
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    //In case a new Account Login
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = email.Split('@')[0],
                            Email = email,
                            google_id = info.ProviderKey,
                            PasswordHash = "Google API"
                        };

                        var createResult = await userManager.CreateAsync(user);
                        if (createResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, "customer");
                            unitOfWork.AccountRepository.UpdateUserLogs(user, "Google Register");
                            createResult = await userManager.AddLoginAsync(user, info);

                            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                            await signInManager.SignInAsync(user, isPersistent: false);

                            return RedirectToAction("SetProfilePic", "Account", new { userId = user.Id });
                        }
                    }

                    if (user != null)
                    {

                        // Clear any existing external cookie
                        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                        // Sign in the user
                        await signInManager.SignInAsync(user, isPersistent: false);
                        unitOfWork.AccountRepository.UpdateUserLogs(user, "Google  Login");
                        unitOfWork.AccountRepository.UpdateLastLog(user.Id);

                        return RedirectToAction("Index", "Switch");   ///////////////////////////////////////////
                    }
                }
                return RedirectToAction(nameof(Login));
            }
        }
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return RedirectToAction("Index", "Landing");
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleDataVM roleDataVM)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    IdentityRole identityRole = new IdentityRole()
                    {
                        Name = roleDataVM.RoleName
                    };
                    IdentityResult result = await roleManager.CreateAsync(identityRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    foreach (var i in result.Errors)
                    {
                        ModelState.AddModelError("", i.Description);
                    }
                }
            }
            return View();
        }


        [HttpGet]
        public IActionResult SetProfilePic(string userId)
        {
            return View(new ProfilePic_DateOfBirth() { UserID = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetProfilePic(ProfilePic_DateOfBirth data)
        {
            if (ModelState.IsValid)
            {
                var condition = await unitOfWork.AccountRepository.SetProfileAndBirthday(data);
                if (condition)
                {
                    return RedirectToAction("Index", "Switch");
                }
            }

            return View(data);
        }

    }
}