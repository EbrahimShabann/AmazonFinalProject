using Final_project.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
namespace Final_project.Controllers
{
    //[Authorize(Roles = "Admin")]

    public class AdminDashboardController : Controller
    {
        private readonly AmazonDBContext _context;

        public AdminDashboardController(AmazonDBContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? from, DateTime? to)
        {
            // Total Users with roles
            var customerRoleId = _context.Roles.Where(r => r.Name == "Customer").Select(r => r.Id).FirstOrDefault();
            var sellerRoleId = _context.Roles.Where(r => r.Name == "Seller").Select(r => r.Id).FirstOrDefault();
            var SellerCount = _context.UserRoles.Count(ur => ur.RoleId == sellerRoleId);

            var customeCount = _context.UserRoles.Count(ur => ur.RoleId == customerRoleId);

            // Product Stats
            var lastMonthDate = DateTime.Now.AddMonths(-1);

            var counttotalProducts = _context.products.Count();
            var countProductsToLastMonth = _context.products.Where(p => p.created_at <= lastMonthDate && (bool)p.is_active).Count();
            var pendingProducts = _context.products.Count(p => (bool)!p.is_approved);
            var productPercetage = countProductsToLastMonth != 0 ? ((counttotalProducts - countProductsToLastMonth) / countProductsToLastMonth) : 0;

            // Pending Sellers: sellers with account not yet active or approved
            var pendingSellers = _context.Users
                .Where(u =>
                    _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId) &&
                    !u.is_active
                ).Count();

            // Support Tickets
            var totalSupportTickets = _context.support_tickets.Count();

            //Percentages 
            var Allsellers = _context.Users
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId)&&u.is_active) 
            .ToList();
            var countAllsellers = Allsellers.Count();

            var sellersToLastMonth = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId) && u.created_at <= lastMonthDate && u.is_active)
            .ToList();
            var countsellersToLastMonth = sellersToLastMonth.Count();
            var AllCustomers = _context.Users
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == customerRoleId) && u.is_active)
            .ToList();
            var countAllCustomers = AllCustomers.Count();
            var customersToLastMonth = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == customerRoleId) && u.created_at <= lastMonthDate && u.is_active)
            .ToList();



            var countcustomersToLastMonth = customersToLastMonth.Count();

            var sellerPercentage = countsellersToLastMonth!=0?((countAllsellers - countsellersToLastMonth) / countsellersToLastMonth):0;
            var customerPercentage = countcustomersToLastMonth != 0 ? ((countAllCustomers - countcustomersToLastMonth) / countcustomersToLastMonth) : 0;

            // Send to view
            ViewBag.productPercetage = productPercetage;
            ViewBag.sellerPercentage = sellerPercentage;
            ViewBag.customerPercentage = customerPercentage;
            ViewBag.TotalCustomers = customeCount;
            ViewBag.TotalSellers =SellerCount;
            ViewBag.TotalProducts = counttotalProducts;
            ViewBag.PendingProducts = pendingProducts;
            ViewBag.PendingSellers = pendingSellers;
            ViewBag.TotalSupportTickets = totalSupportTickets;
            ViewBag.SupportTickets = _context.support_tickets.Where(t => !t.is_deleted).OrderByDescending(t => t.created_at).Take(5).ToList();
            // Chart: New Customers & Sellers per Month
            var usersByMonth = _context.Users
                .Where(u => u.created_at.Year == DateTime.Now.Year)
                .GroupBy(u => new { u.created_at.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Customers = g.Count(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == customerRoleId)),
                    Sellers = g.Count(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId))
                })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.MonthLabels = usersByMonth.Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month)).ToList();
            ViewBag.MonthlyCustomers = usersByMonth.Select(x => x.Customers).ToList();
            ViewBag.MonthlySellers = usersByMonth.Select(x => x.Sellers).ToList();

            // For Pending Chart
            ViewBag.PendingChartData = new List<int> { pendingSellers, pendingProducts };

            return View();
        }
    }
}
