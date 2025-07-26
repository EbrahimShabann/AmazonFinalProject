using Final_project.Models;
using Final_project.Repository;
using Final_project.Services.Customer;
using Final_project.ViewModel.Customer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace Final_project.Areas.Customer.Controllers
{
    [Area("Customer")]
  
    public class ProfileController : Controller
    {
        private readonly UnitOfWork uof;
        private readonly UserManager<ApplicationUser> userManager;

        public ProfileController(UnitOfWork uof, UserManager<ApplicationUser> _userManager)
        {
            this.uof = uof;
            userManager = _userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Orders(string dateFilter,string statusFilter,string search, int page=1,int size=10)
        {
            string userId = "c4";//User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = uof.OrderRepo.getAll().Where(o => o.buyer_id == userId);

            // Filter based on delivered_at
            if (!string.IsNullOrEmpty(dateFilter))
            {
                DateTime now = DateTime.Now;

                switch (dateFilter)
                {
                    case "30":
                        orders = orders.Where(o => o.delivered_at != null && o.delivered_at >= now.AddDays(-30));
                        break;
                    case "90":
                        orders = orders.Where(o => o.delivered_at != null && o.delivered_at >= now.AddDays(-90));
                        break;
                    case "2023":
                        orders = orders.Where(o => o.delivered_at != null && o.delivered_at.Value.Year == 2023);
                        break;
                    case "2022":
                        orders = orders.Where(o => o.delivered_at != null && o.delivered_at.Value.Year == 2022);
                        break;
                    default:
                        orders = uof.OrderRepo.getAll().Where(o => o.buyer_id == userId);
                        break;
                }
            };
            if(!string.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter)
                {
                    case "shipped":
                        orders = orders.Where(o => o.status.ToLower() == "shipped");
                        break;
                    case "pending":
                        orders = orders.Where(o => o.status.ToLower() == "pending");
                        break;
                    case "delivered":
                        orders = orders.Where(o => o.status == "delivered");
                        break;
                    case "cancelled":
                        orders = orders.Where(o => o.status == "cancelled");
                        break;
                    default:
                        orders = uof.OrderRepo.getAll().Where(o => o.buyer_id == userId);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(search))
            {
                search = search?.ToLower();

                orders = orders.Where(o =>
                    (!string.IsNullOrEmpty(o.shipping_address) && o.shipping_address.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(o.tracking_number) && o.tracking_number.ToLower().Contains(search)) ||
                    o.total_amount.ToString().Contains(search));

            }

            var pagedOrders = orders.ToPagedResult(page, size);

            //request from ajax
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_OrdersList", pagedOrders);
            }

               return View(pagedOrders);
        }
      
        public IActionResult orderDetails(string id)
        {
            var order = uof.OrderRepo.getById(id);
            var orderItems = uof.OrderRepo.GetOrderItemsOfOrder(id);
            var orderHistory = uof.OrderRepo.GetOrderHistoryByOrderId(id);
            var orderDetailsVM = new OrderDetailsVM
            {
                Order = order,
                OrderItems = orderItems,
                OrderHistory= orderHistory,
            };
            if (order == null)
            {
                return NotFound();
            }
            return View(orderDetailsVM);
        }

        public async Task<IActionResult> LoginAndSecurity()
        {
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            //var user = await userManager.GetUserAsync(User);
            //if (user == null)
            //    RedirectToAction("Login", "Account");
            return View(user);
        }

        public async Task<IActionResult> EditName()
        {
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            var model = new UpdateNameViewModel()
            {
                FullName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditName(UpdateNameViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            //var user = await userManager.GetUserAsync(User);
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            user.UserName = model.FullName;
            await userManager.UpdateAsync(user);
            return RedirectToAction("LoginAndSecurity");
        }

        public async Task<IActionResult> EditEmail()
        {
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            var model = new UpdateEmailViewModel
            {
                NewEmail = user.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmail(UpdateEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            //var user = await userManager.GetUserAsync(User);
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            await userManager.SetEmailAsync(user, model.NewEmail);
            return RedirectToAction("LoginAndSecurity");
        }

        public async Task<IActionResult> EditPhone()
        {
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            var model = new UpdatePhoneViewModel
            {
                PhoneNumber = user.PhoneNumber
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPhone(UpdatePhoneViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            //var user = await userManager.GetUserAsync(User);
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            await userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            return RedirectToAction("LoginAndSecurity");
        }

        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            //var user = await userManager.GetUserAsync(User);
            var user = await userManager.FindByEmailAsync("customer1@example.com");
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }
            return RedirectToAction("LoginAndSecurity");
        }
    }
}
