using Final_project.Models;
using Final_project.Repository;
using Final_project.Services.Customer;
using Final_project.ViewModel.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Final_project.Areas.Customer.Controllers
{
    //[Authorize(Roles = "Customer")]
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
        public IActionResult Orders(string dateFilter, string statusFilter, string search, int page = 1, int size = 10)
        {
            string userId = "c1";//User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            }
            ;
            if (!string.IsNullOrEmpty(statusFilter))
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
                OrderHistory = orderHistory,
            };
            if (order == null)
            {
                return NotFound();
            }
            return View(orderDetailsVM);
        }

        public IActionResult CancelOrder(string orderId)
        {
            var order = uof.OrderRepo.getById(orderId);
            var orderHistory = uof.OrderRepo.GetOrderHistoryByOrderId(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.status = "cancelled";

            if (orderHistory == null)
            {
                //add order history 
                var newOrderHistory = new order_history
                {
                    id = Guid.NewGuid().ToString(),
                    order_id = order.id,
                    status = "Pending",
                    notes = "Order has been created",
                    changed_at = DateTime.Now,
                    changed_by = "c4" //User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                uof.OrderRepo.AddOrderHistory(newOrderHistory);
            }
            else
            {
                //update order history if it exists
                orderHistory.status = "cancelled";
                orderHistory.notes = "Order has been cancelled by user";
                orderHistory.changed_at = DateTime.Now;
                orderHistory.changed_by = "c4"; //User.FindFirstValue(ClaimTypes.NameIdentifier)
                uof.OrderRepo.UpdateOrderHistory(orderHistory);
            }

            uof.OrderRepo.Update(order);
            uof.save();
            return Ok();
        }

        public IActionResult Messages(string filter, string search, int page = 1, int size = 5)
        {
            var userId = "c4";//User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }
            var messages = uof.MessageRepo.getBySenderId(userId);
            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower())
                {
                    case "unread":
                        messages = messages.Where(m => m.is_read == false).ToList();
                        break;
                    case "read":
                        messages = messages.Where(m => m.is_read == true).ToList();
                        break;
                    default:
                        messages = uof.MessageRepo.getBySenderId(userId);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(search))
            {
                messages = messages.Where(m => m.message.Contains(search)).ToList();
            }

            var pagedMessages = messages.ToPagedResult(page, size);


            //request from ajax
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_MessagesList", pagedMessages);
            }
            return View(pagedMessages);
        }

        public IActionResult DeleteSelected(List<string> selectedIds , string msgId)
        {
            if(!string.IsNullOrEmpty(msgId))
            {
               selectedIds.Add(msgId);
            }
            if (selectedIds == null || selectedIds.Count == 0)
            {
                return BadRequest("No messages selected for deletion.");
            }
            var userId = "c4";//User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }
            foreach (var id in selectedIds)
            {
                var message = uof.MessageRepo.getById(id);
                if (message != null && message.sender_id == userId)
                {
                    uof.MessageRepo.Delete(message);
                   
                }
                else 
                {
                    return BadRequest();
                }
            }

            uof.save();
            return Ok("Selected messages deleted successfully.");
        }
   
        public IActionResult staticPages(string page)
        {
            if (page == "app")
            {
                return View("AmazonApp");
            }          
            else if (page == "contact")
            {
                var userId = "c4";//User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.userOrders=uof.OrderRepo.getAll().Where(o => o.buyer_id == userId).ToList();
                return View("ContactUs");
            }
            else if (page == "faq")
            {
                return View("FAQ");
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet]
        public IActionResult CreateReturn(string id)
        {
            var orderItem = uof.OrderRepo.GetOrderItemById(id);
            if (orderItem == null)
            {
                TempData["error"] = "Order item not found.";
                return RedirectToAction("orderDetails",orderItem.order_id);
            }
            var revertVM = new ordersReverted
            {
                order_itemId = orderItem.id,
                Order_Item=uof.OrderRepo.GetOrderItemById(orderItem.id),
                orderId = orderItem.order_id,
                Order = uof.OrderRepo.getById(orderItem.order_id),
                RevertDate = DateTime.Now,
                Reason = "",
                Notes = ""
            };
            return PartialView("_revertOrder", revertVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReturn(ordersReverted revertVM)
        {
            revertVM.Order = uof.OrderRepo.getById(revertVM.orderId);
            revertVM.Order_Item = uof.OrderRepo.GetOrderItemById(revertVM.order_itemId);
            if (ModelState.IsValid)
            {
                var orderItem = uof.OrderRepo.GetOrderItemById(revertVM.order_itemId);
                if (orderItem == null)
                {
                    TempData["error"] = "Order item not found.";
                    return RedirectToAction("orderDetails", orderItem.order_id);
                }
                // Create new revert record
                var revertRecord = new ordersReverted
                {

                    orderId = orderItem.order_id,
                    order_itemId = orderItem.id,
                    RevertDate = DateTime.Now,
                    Reason = revertVM.Reason,
                    Notes = revertVM.Notes
                };

               var orderHistory= uof.OrderRepo.GetOrderHistoryByOrderId(revertVM.orderId);
                orderHistory.status = revertVM.Reason;

                uof.OrderRepo.AddReturnOrder(revertRecord);
                uof.OrderRepo.UpdateOrderHistory(orderHistory);
                uof.save();
                TempData["success"] = "Order item reverted successfully.";
                return RedirectToAction("orderDetails", orderItem.order_id);
            }
          
            TempData["error"] = "Invalid data.";
            return PartialView("_revertOrder", revertVM);
        }

    }
}
