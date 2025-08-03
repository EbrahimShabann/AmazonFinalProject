using Final_project.Models;
using Final_project.Repository;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Final_project.Controllers
{
    [Authorize(Roles = "admin,superadmin")]

    public class AdminDashboardController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardController(UnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager)
        {
            this.unitOfWork = unitOfWork;
            this._userManager = _userManager;

        }

        public async Task<IActionResult> IndexAsync(DateTime? from, DateTime? to)
        {

            int customeCount = (await _userManager.GetUsersInRoleAsync("Customer")).Count;
            int SellerCount = (await _userManager.GetUsersInRoleAsync("Seller")).Count;
            var sellers = await _userManager.GetUsersInRoleAsync("Seller");
            var customers = (await _userManager.GetUsersInRoleAsync("Customer"));

            // Product Stats
            var lastMonthDate = DateTime.Now.AddMonths(-1);

            int counttotalProducts = unitOfWork.ProductRepository.GetAll(p => !p.is_deleted).ToList().Count;
            var countProductsToLastMonth = unitOfWork.ProductRepository.GetAll(p => p.created_at <= lastMonthDate && (bool)p.is_active).ToList().Count;
            var pendingProducts = unitOfWork.ProductRepository.GetAll(p => (bool)!p.is_approved & (bool)p.is_active & !p.is_deleted).ToList().Count;
            var productPercetage = countProductsToLastMonth != 0 ? ((counttotalProducts - countProductsToLastMonth) / countProductsToLastMonth) : 0;

            // Pending Sellers: sellers with account not yet active or approved
            var pendingSellers = sellers.Where(s => !s.is_active & !s.is_deleted).Count();



            //Percentages 
            var Allsellers = sellers.Where(s => s.is_active)
            .ToList();
            var countAllsellers = Allsellers.Count();

            var sellersToLastMonth = sellers.Where(u => u.created_at <= lastMonthDate && u.is_active)
            .ToList();
            var countsellersToLastMonth = sellersToLastMonth.Count();
            var AllCustomers = customers
            .Where(u => u.is_active)
            .ToList();
            var countAllCustomers = AllCustomers.Count();
            var customersToLastMonth = customers.Where(u => u.created_at <= lastMonthDate && u.is_active)
            .ToList();



            var countcustomersToLastMonth = customersToLastMonth.Count();

            var sellerPercentage = countsellersToLastMonth != 0 ? ((countAllsellers - countsellersToLastMonth) / countsellersToLastMonth) : 0;
            var customerPercentage = countcustomersToLastMonth != 0 ? ((countAllCustomers - countcustomersToLastMonth) / countcustomersToLastMonth) : 0;

            // Send to view
            ViewBag.productPercetage = productPercetage;
            ViewBag.sellerPercentage = sellerPercentage;
            ViewBag.customerPercentage = customerPercentage;
            ViewBag.TotalCustomers = customeCount;
            ViewBag.TotalSellers = SellerCount;
            ViewBag.TotalProducts = counttotalProducts;
            ViewBag.PendingProducts = pendingProducts;
            ViewBag.PendingSellers = pendingSellers;
            //ViewBag.TotalSupportTickets = totalSupportTickets;
            //ViewBag.SupportTickets = _context.support_tickets.Where(t => !t.is_deleted).OrderByDescending(t => t.created_at).Take(5).ToList();
            // Chart: New Customers & Sellers per Month
            var usersByMonth = unitOfWork.UserRepository.GetAll()
                .Where(u => u.created_at.Year == DateTime.Now.Year)
                .GroupBy(u => new { u.created_at.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Customers = customers.Count,
                    Sellers = sellers.Count
                })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.MonthLabels = usersByMonth.Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month)).ToList();
            ViewBag.MonthlyCustomers = usersByMonth.Select(x => x.Customers).ToList();
            ViewBag.MonthlySellers = usersByMonth.Select(x => x.Sellers).ToList();

            // For Pending Chart
            ViewBag.PendingChartData = new List<int> { pendingSellers, pendingProducts };
            ViewBag.ActivePage = "Dashboard";
            return View();
        }
    }
}