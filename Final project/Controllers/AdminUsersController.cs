using Final_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class AdminUsersController : Controller
    {
       
            private readonly AmazonDBContext _context;

            public AdminUsersController(AmazonDBContext context)
            {
                _context = context;
            }

            public IActionResult GetUsersByRole(string roleName, DateTime? fromDate, DateTime? toDate, string search, int page = 1, int pageSize = 10)
            {

                var roleId = _context.Roles
                    .Where(r => r.Name.ToLower() == roleName.ToLower())
                    .Select(r => r.Id)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(roleId)) return NotFound("Role not found");

                var query = _context.Users
                    .Where(u =>
                        _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId)
                    );

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u =>
                        u.UserName.Contains(search) ||
                        u.Email.Contains(search)
                    );
                }

                if (fromDate.HasValue)
                    query = query.Where(u => u.created_at >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(u => u.created_at <= toDate.Value);

                var totalUsers = query.Count();
                var users = query
                    .OrderByDescending(u => u.created_at)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.Role = roleName;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Total = totalUsers;
                ViewBag.Search = search;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

                return View("RoleUsersList", users);
            }

            public IActionResult Customers(DateTime? fromDate, DateTime? toDate, string search, int page = 1)
            {
                return GetUsersByRole("Customer", fromDate, toDate, search, page);
            }

            public IActionResult Sellers(DateTime? fromDate, DateTime? toDate, string search, int page = 1)
            {
                return GetUsersByRole("Seller", fromDate, toDate, search, page);
            }

            public IActionResult Admins(DateTime? fromDate, DateTime? toDate, string search, int page = 1)
            {
                return GetUsersByRole("Admin", fromDate, toDate, search, page);
            }

            public IActionResult ToggleActive(string id)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                user.is_active = !user.is_active;
                _context.SaveChanges();

                return Redirect(Request.Headers["Referer"].ToString());
            }

            public IActionResult Details(string id)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                return View(user);
            }
    }

}
