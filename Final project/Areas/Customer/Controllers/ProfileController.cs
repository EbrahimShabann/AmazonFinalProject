using Final_project.Repository;
using Final_project.Services.Customer;
using Final_project.ViewModel.Customer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Final_project.Areas.Customer.Controllers
{
    [Area("Customer")]
  
    public class ProfileController : Controller
    {
        private readonly UnitOfWork uof;

        public ProfileController(UnitOfWork uof)
        {
            this.uof = uof;
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

            var pagedOrders=orders.ToPagedResult(page,size);
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
    }
}
