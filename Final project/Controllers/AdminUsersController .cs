using Final_project.Models;
using Final_project.ViewModel.CreateUserViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


public class AdminUsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminUsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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


        if (model.imgFile != null && model.imgFile.Length > 0)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
            Directory.CreateDirectory(uploads);

            var fileName = Path.GetFileName(model.imgFile.FileName);
            var filePath = Path.Combine(uploads, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await model.imgFile.CopyToAsync(stream);

            // Point your model at the saved path
        }
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            date_of_birth = model.birthdate,
            profile_picture_url = model.imgFile != null ?Path.GetFileName(model.imgFile.FileName) : null

        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(model.SelectedRole))
            {
                ModelState.AddModelError("", "Selected role does not exist.");
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.SelectedRole);
            TempData["Success"] = "User added successfully!";
            return RedirectToAction("Index", "AdminDashboard");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }
}
