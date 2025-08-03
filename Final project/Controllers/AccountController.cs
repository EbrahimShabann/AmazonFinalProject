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
using Final_project.Services.SmsService;
using Final_project.Services.DeviceService;
using Final_project.Services.TwoFactorService;
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
        private readonly ISmsService smsService;
        private readonly IDeviceService deviceService;
        private readonly ITwoFactorService twoFactorService;
        private readonly IConfiguration _configuration;

        public AccountController(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
                         IEmailService emailService, ISmsService smsService, IDeviceService deviceService,
                         ITwoFactorService twoFactorService,IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
            this.smsService = smsService;
            this.deviceService = deviceService;
            this.twoFactorService = twoFactorService;
            this._configuration = configuration;
        }

        #region Login and Registration

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
                        // Check if user has phone number but it's not confirmed
                        if (!string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumberConfirmed != "true")
                        {
                            // Redirect to phone verification first
                            HttpContext.Session.SetString("PendingUserId", user.Id);
                            HttpContext.Session.SetString("RememberMe", loginData.RememberMe.ToString());
                            HttpContext.Session.SetString("LoginPurpose", "PhoneVerification");

                            return RedirectToAction("VerifyPhoneNumber", new { purpose = "Login" });
                        }

                        // Check if user has 2FA enabled and phone is confirmed
                        if (user.TwoFactorEnabled && !string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumberConfirmed == "true")
                        {
                            // Check if this is a new/untrusted device
                            bool isNewDevice = await deviceService.IsNewDeviceAsync(user.Id, HttpContext);

                            if (isNewDevice)
                            {
                                // Store login attempt in session
                                HttpContext.Session.SetString("PendingUserId", user.Id);
                                HttpContext.Session.SetString("RememberMe", loginData.RememberMe.ToString());
                                HttpContext.Session.SetString("LoginPurpose", "TwoFactor");

                                // Send SMS verification for new device
                                await twoFactorService.SendLoginVerificationAsync(user.Id, user.PhoneNumber);

                                // Log the new device attempt
                                unitOfWork.AccountRepository.UpdateUserLogs(user, "New device login attempt - SMS sent");

                                return RedirectToAction("VerifyTwoFactor", new { purpose = "Login" });
                            }
                        }

                        // Proceed with normal login for trusted devices or users without 2FA
                        await signInManager.SignInAsync(user, loginData.RememberMe);
                        var data = unitOfWork.AccountRepository.UpdateUserLogs(user, "Login successful");
                        unitOfWork.AccountRepository.UpdateLastLog(user.Id);

                        // Update device information
                        await deviceService.GetOrCreateDeviceAsync(user.Id, HttpContext);

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
                    PhoneNumberConfirmed = "false",
                    TwoFactorEnabled = false,
                };

                IdentityResult created = await userManager.CreateAsync(user, registerData.Password);
                if (created.Succeeded)
                {
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
                await signInManager.SignInAsync(user, false);
                unitOfWork.AccountRepository.UpdateUserLogs(user, "Email confirmed");

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

        #endregion

        #region Profile Setup

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
                    // If phone number was provided, redirect to phone verification
                    if (!string.IsNullOrEmpty(data.PhoneNumber))
                    {
                        HttpContext.Session.SetString("PendingUserId", data.UserID);
                        HttpContext.Session.SetString("LoginPurpose", "PhoneVerification");
                        return RedirectToAction("VerifyPhoneNumber", new { purpose = "Registration" });
                    }

                    return RedirectToAction("Index", "Switch");
                }
            }

            return View(data);
        }

        #endregion

        #region Phone Number Verification

        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string purpose)
        {
            var userId = HttpContext.Session.GetString("PendingUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
            {
                return RedirectToAction("Login");
            }

            // Send SMS verification code
            var code = await twoFactorService.GenerateCodeAsync(userId, "PhoneVerification");
            var sent = await smsService.SendVerificationCodeAsync(user.PhoneNumber, code);

            if (!sent)
            {
                TempData["ErrorMessage"] = "Failed to send SMS verification code. Please try again.";
                return RedirectToAction("Login");
            }

            unitOfWork.AccountRepository.UpdateUserLogs(user, $"Phone verification SMS sent for {purpose}");

            var model = new VerifyPhoneNumberVM
            {
                UserId = userId,
                PhoneNumber = user.PhoneNumber,
                Purpose = purpose,
                MaskedPhoneNumber = GetMaskedPhoneNumber(user.PhoneNumber)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberVM model)
            {
            if (ModelState.IsValid)
            {
                var isValid = await twoFactorService.ValidateCodeAsync(model.UserId, model.Code, "PhoneVerification");

                if (isValid)
                {
                    var user = await userManager.FindByIdAsync(model.UserId);
                    if (user != null)
                    {
                        // Mark phone number as confirmed
                        await unitOfWork.AccountRepository.UpdatePhoneConfirmationAsync(user.Id, true);

                        unitOfWork.AccountRepository.UpdateUserLogs(user, "Phone number verified successfully");

                        var loginPurpose = HttpContext.Session.GetString("LoginPurpose");

                        if (loginPurpose == "PhoneVerification" && model.Purpose == "Login")
                        {
                            // After phone verification during login, check for 2FA
                            if (user.TwoFactorEnabled)
                            {
                                bool isNewDevice = await deviceService.IsNewDeviceAsync(user.Id, HttpContext);
                                if (isNewDevice)
                                {
                                    HttpContext.Session.SetString("LoginPurpose", "TwoFactor");
                                    await twoFactorService.SendLoginVerificationAsync(user.Id, user.PhoneNumber);
                                    return RedirectToAction("VerifyTwoFactor", new { purpose = "Login" });
                                }
                            }

                            // Sign in user after phone verification
                            var rememberMe = bool.Parse(HttpContext.Session.GetString("RememberMe") ?? "false");
                            await signInManager.SignInAsync(user, rememberMe);
                            await deviceService.GetOrCreateDeviceAsync(user.Id, HttpContext);
                            unitOfWork.AccountRepository.UpdateLastLog(user.Id);

                            ClearLoginSession();
                            return RedirectToAction("Index", "Switch");
                        }
                        else if (model.Purpose == "Registration")
                        {
                            // After registration phone verification, ask if they want to enable 2FA
                            return RedirectToAction("EnableTwoFactorPrompt");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid verification code.");
                }
            }

            // Reload the model for display
            var userForModel = await userManager.FindByIdAsync(model.UserId);
            model.MaskedPhoneNumber = GetMaskedPhoneNumber(userForModel?.PhoneNumber);
            return View(model);
        }

        [HttpGet]
        public IActionResult EnableTwoFactorPrompt()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorPrompt(bool enableTwoFactor)
        {
            var userId = HttpContext.Session.GetString("PendingUserId");
            if (!string.IsNullOrEmpty(userId))
            {
                if (enableTwoFactor)
                {
                    await unitOfWork.AccountRepository.UpdateTwoFactorEnabledAsync(userId, true);
                    var user = await userManager.FindByIdAsync(userId);
                    unitOfWork.AccountRepository.UpdateUserLogs(user, "Two-factor authentication enabled during registration");
                }

                ClearLoginSession();
            }

            return RedirectToAction("Index", "Switch");
        }

        [HttpPost]
        public async Task<IActionResult> ResendPhoneVerification()
        {
            var userId = HttpContext.Session.GetString("PendingUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Session expired. Please try again." });
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
            {
                var code = await twoFactorService.GenerateCodeAsync(userId, "PhoneVerification");
                var sent = await smsService.SendVerificationCodeAsync(user.PhoneNumber, code);

                if (sent)
                {
                    unitOfWork.AccountRepository.UpdateUserLogs(user, "Phone verification SMS resent");
                }

                return Json(new { success = sent, message = sent ? "Code sent!" : "Failed to send SMS." });
            }

            return Json(new { success = false, message = "Unable to send verification code." });
        }

        #endregion

        #region Two Factor Authentication

        [HttpGet]
        public IActionResult VerifyTwoFactor(string purpose)
        {
            var userId = HttpContext.Session.GetString("PendingUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var model = new TwoFactorVerificationVM
            {
                UserId = userId,
                Purpose = purpose,
                RememberMe = bool.Parse(HttpContext.Session.GetString("RememberMe") ?? "false")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactor(TwoFactorVerificationVM model)
        {
            if (ModelState.IsValid)
            {
                var isValid = await twoFactorService.ValidateCodeAsync(model.UserId, model.Code, model.Purpose);

                if (isValid)
                {
                    var user = await userManager.FindByIdAsync(model.UserId);
                    if (user != null)
                    {
                        // Sign in the user
                        await signInManager.SignInAsync(user, model.RememberMe);

                        // Mark device as trusted and update device info
                        var device = await deviceService.GetOrCreateDeviceAsync(user.Id, HttpContext);
                        await deviceService.MarkDeviceAsTrustedAsync(device.Id);

                        // Log successful verification
                        unitOfWork.AccountRepository.UpdateUserLogs(user, "Two-factor authentication successful");
                        unitOfWork.AccountRepository.UpdateLastLog(user.Id);

                        ClearLoginSession();
                        return RedirectToAction("Index", "Switch");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid verification code.");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult EnableTwoFactor()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactor(EnableTwoFactorVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Validate the verification code
                    var isValid = await twoFactorService.ValidateCodeAsync(userId, model.VerificationCode, "EnableTwoFactor");

                    if (isValid)
                    {
                        // Enable 2FA and update phone number
                        user.TwoFactorEnabled = true;
                        user.PhoneNumber = model.PhoneNumber;
                        user.PhoneNumberConfirmed = "true";

                        await userManager.UpdateAsync(user);

                        unitOfWork.AccountRepository.UpdateUserLogs(user, "Two-factor authentication enabled");

                        TempData["SuccessMessage"] = "Two-factor authentication has been enabled successfully.";
                        return RedirectToAction("Profile", "User");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid verification code.");
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendPhoneVerification(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Json(new { success = false, message = "Phone number is required." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var code = await twoFactorService.GenerateCodeAsync(userId, "EnableTwoFactor");
            var sent = await smsService.SendVerificationCodeAsync(phoneNumber, code);

            return Json(new { success = sent, message = sent ? "Verification code sent!" : "Failed to send SMS." });
        }

        [Authorize]
        public async Task<IActionResult> DisableTwoFactor()
        {
            if (Request.Method == "GET")
            {
                return View();
            }
            else if (Request.Method == "POST")
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    user.TwoFactorEnabled = false;
                    await userManager.UpdateAsync(user);
                    unitOfWork.AccountRepository.UpdateUserLogs(user, "Two-factor authentication disabled");
                    TempData["SuccessMessage"] = "Two-factor authentication has been disabled.";
                }

                return RedirectToAction("Profile", "User");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendTwoFactorCode()
        {
            var userId = HttpContext.Session.GetString("PendingUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Session expired. Please try logging in again." });
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
            {
                var sent = await twoFactorService.SendLoginVerificationAsync(userId, user.PhoneNumber);
                if (sent)
                {
                    unitOfWork.AccountRepository.UpdateUserLogs(user, "Two-factor code resent");
                }
                return Json(new { success = sent, message = sent ? "Code sent!" : "Failed to send SMS." });
            }

            return Json(new { success = false, message = "Unable to send verification code." });
        }

        #endregion

        #region Device Management

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ManageDevices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var devices = await unitOfWork.AccountRepository.GetUserDevicesAsync(userId);

            return View(devices);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveDevice(int deviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var device = await unitOfWork.AccountRepository.GetDeviceAsync(deviceId);

            if (device != null && device.UserId == userId)
            {
                await unitOfWork.AccountRepository.RemoveDeviceAsync(deviceId);
                var user = await userManager.FindByIdAsync(userId);
                unitOfWork.AccountRepository.UpdateUserLogs(user, $"Device removed: {device.DeviceName}");
            }

            return RedirectToAction("ManageDevices");
        }

        #endregion

        #region External Login (Google OAuth)

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                redirectUrl);

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
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = email.Split('@')[0],
                            Email = email,
                            google_id = info.ProviderKey,
                            PasswordHash = "Google API",
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = "false",
                            TwoFactorEnabled = false
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
                        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                        await signInManager.SignInAsync(user, isPersistent: false);
                        unitOfWork.AccountRepository.UpdateUserLogs(user, "Google Login");
                        unitOfWork.AccountRepository.UpdateLastLog(user.Id);

                        return RedirectToAction("Index", "Switch");
                    }
                }
                return RedirectToAction(nameof(Login));
            }
        }

        #endregion

        #region Password Reset

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

                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

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

        #endregion

        #region Role Management

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

        #endregion

        #region Miscellaneous

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return RedirectToAction("Index", "Landing");
        }

        [HttpGet]
        public IActionResult WatingforAdminApproval()
        {
            return View();
        }

        #endregion

        #region Email Connection Testing

        [HttpGet]
        public async Task<IActionResult> TestEmailConnection()
        {
            try
            {
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

                if (string.IsNullOrEmpty(password) || password.Length < 10)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Password appears to be invalid or too short",
                        config = configDetails
                    });
                }

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

        #endregion

        #region Helper Methods

        private string GetMaskedPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 4)
                return phoneNumber;

            return phoneNumber.Substring(0, 4) + "****" + phoneNumber.Substring(phoneNumber.Length - 2);
        }

        private void ClearLoginSession()
        {
            HttpContext.Session.Remove("PendingUserId");
            HttpContext.Session.Remove("RememberMe");
            HttpContext.Session.Remove("LoginPurpose");
        }

        #endregion

     
    }
}