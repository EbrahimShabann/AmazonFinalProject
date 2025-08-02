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
using System.Text.Encodings.Web;
using Final_project.Services.EmailService;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Final_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailService emailService;

        public AccountController(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
                         IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
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
                    // Check if email is confirmed
                    if (!await userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Please verify your email before logging in.");
                        return View(loginData);
                    }

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
                    // Use the selected role instead of hardcoded "customer"
                    await userManager.AddToRoleAsync(user, registerData.Role);
                    unitOfWork.AccountRepository.UpdateUserLogs(user, $"Register as {registerData.Role}");

                    // Generate email confirmation token
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    // Send verification email
                    await emailService.SendVerificationEmailAsync(user.Email, confirmationLink);

                    return RedirectToAction("RegisterConfirmation");
                }

                foreach (var item in created.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View("Register", registerData);
        }
        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Invalid email confirmation request.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Optionally sign in the user after email confirmation
                await signInManager.SignInAsync(user, false);
                return RedirectToAction("SetProfilePic", "Account", new { userId = user.Id });
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("RegisterConfirmation");
            }

            if (await userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToAction("Login");
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            await emailService.SendVerificationEmailAsync(user.Email, confirmationLink);

            return RedirectToAction("RegisterConfirmation");
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
                            PasswordHash = "Google API",
                            EmailConfirmed=true,
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
        [HttpGet]
        public IActionResult WatingforAdminApproval()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                // Don't reveal that the user does not exist or is not confirmed
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // Generate password reset token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                // Create password reset link
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                // Send password reset email
                await emailService.SendPasswordResetEmailAsync(user.Email, resetLink, user.UserName);

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Invalid password reset request.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new ResetPasswordVM
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                // Log the password reset activity
                unitOfWork.AccountRepository.UpdateUserLogs(user, "Password Reset");

                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TestEmailConnection()
        {
            try
            {
                // Cast to your concrete implementation to access TestConnectionAsync
                var emailServiceImpl = emailService as Final_project.Services.EmailService.EmailService;
                if (emailServiceImpl != null)
                {
                    bool isConnected = await emailServiceImpl.TestConnectionAsync();
                    return Json(new
                    {
                        success = isConnected,
                        message = isConnected ? "Email connection successful!" : "Email connection failed - check logs"
                    });
                }
                return Json(new { success = false, message = "Could not test connection" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestEmailConnectionDetailed()
        {
            try
            {
                var config = HttpContext.RequestServices.GetService<IConfiguration>();
                var smtpServer = config["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(config["EmailSettings:SmtpPort"]);
                var username = config["EmailSettings:Username"];
                var password = config["EmailSettings:Password"];

                // Show configuration details (mask password)
                var configDetails = new
                {
                    SmtpServer = smtpServer,
                    SmtpPort = smtpPort,
                    Username = username,
                    PasswordLength = password?.Length ?? 0,
                    PasswordStart = password?.Length > 4 ? password.Substring(0, 4) + "****" : "****",
                    PasswordHasSpaces = password?.Contains(" ") ?? false,
                    PasswordIsAppPasswordLength = password?.Replace(" ", "").Length == 16
                };

                // Return config first if there are obvious issues
                if (string.IsNullOrEmpty(password) || password.Length < 10)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Password appears to be invalid or too short",
                        config = configDetails
                    });
                }

                // Direct test with detailed error reporting
                using var client = new MailKit.Net.Smtp.SmtpClient();

                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.DisconnectAsync(true);

                return Json(new
                {
                    success = true,
                    message = "Connection successful!",
                    config = configDetails
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}