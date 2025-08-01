using Final_project.Hubs;
using Final_project.Models;
using Final_project.Repository;
using Final_project.Services.Customer;
using Final_project.ViewModel.Cart;
using Final_project.ViewModel.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    //[Authorize(Roles = "Customer")]

    public class ProductController : Controller
    {
        private readonly UnitOfWork uof;
        private readonly IHubContext<SellerOrdersHub> hub;

        public ProductController(UnitOfWork uof,IHubContext<SellerOrdersHub> hub)
        {
            this.uof = uof;
            this.hub = hub;
        }


        public IActionResult Details(string id)
        {
            ProductsVM product = uof.ProductRepository.getProductsWithImagesAndRating().SingleOrDefault(p => p.id == id);
            if (product == null)
            {
                return NotFound();
            }
            product.ExistingImages = uof.ProductRepository.GetProduct_Images(id);
            product.reviews = uof.ProductRepository.getProductReviews(id);
            product.RecommendedProducts = uof.ProductRepository.getProductsWithImagesAndRating().Where(p => p.category_id == product.category_id).ToList();
            return View(product);
        }

        [Authorize]
        public IActionResult addReview(product_review reviewVM)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                //check if the user has purchased the product if true then he can review it
                var userOrderItem = uof.OrderItemRepository.GetAll()
                    .FirstOrDefault(oi => oi.product_id == reviewVM.product_id && oi.order.buyer_id == userId);
                if(userOrderItem == null)
                {
                    TempData["error"] = "You have to try this product first!";
                    return RedirectToAction("Details", new { id = reviewVM.product_id });
                }
                //check if the user has already reviewed the product
                var existingReview = uof.ProductRepository.getProductReviews(reviewVM.product_id)
                    .FirstOrDefault(r => r.user_id == userId);
                if (existingReview != null)
                {
                    TempData["error"] = "You can't review twice!";
                    return RedirectToAction("Details", new { id = reviewVM.product_id });
                }
                if (reviewVM.rating < 1 || reviewVM.rating > 5)
                {
                    TempData["error"] = "Rating must be between 1 and 5.";
                    return RedirectToAction("Details", new { id = reviewVM.product_id });
                }
                var review = new product_review
                {
                    id = Guid.NewGuid().ToString(),
                    product_id = reviewVM.product_id,
                    title = reviewVM.title,
                    comment = reviewVM.comment,
                    rating = reviewVM.rating,
                    user_id = userId, 
                    created_at = DateTime.Now,

                };
                if (reviewVM.rating > 2)
                {
                    review.is_verified_purchase = true;
                }
                else
                {
                    review.is_verified_purchase = false;
                }
                uof.ProductRepository.addReview(review);
                uof.save();
                TempData["success"] = "Thank you for your review!";
                return RedirectToAction("Details", new { id = reviewVM.product_id });
            }
            else
            {
                TempData["error"] = "Please fill all required fields";
                return RedirectToAction("Details", new { id = reviewVM.product_id });
            }
        }


        [HttpGet]
        [Authorize]
        public IActionResult CheckOut(List<CartVM> cartVM)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;        //customerId
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "You must be logged in to checkout.";
                return RedirectToAction("Login", "Account");
            }
            var userName = User.Identity.Name;
            var userPhone = uof.UserRepository.getById(userId).PhoneNumber;


            var newOrder = new CheckOutVM
            {
                UserName = userName,
                UserPhone = userPhone,
                Carts = new List<CartVM>(),
            };


            var lastOrder = uof.OrderRepo.getAll().OrderByDescending(o => o.order_date)
                                .FirstOrDefault(o => o.buyer_id == userId); //last order
            if (lastOrder != null)
            {
                newOrder.shipping_address = lastOrder.shipping_address;
                newOrder.payment_method = lastOrder.payment_method;
            }

            if (cartVM != null && cartVM.Any())
            {
                //Coming from cart with multiple products
                foreach (var cart in cartVM)
                {
                    var product = uof.ProductRepository.getById(cart.ProductId);

                    if (product == null)
                    {
                        TempData["error"] = "Product not found.";
                        return RedirectToAction("Index", "Landing");
                    }
                    if (cart.Quantity <= 0)
                    {
                        TempData["error"] = "Quantity must be greater than zero.";
                        return RedirectToAction("Index", "Cart");
                    }
                    if (string.IsNullOrEmpty(cart.ProductColor)) cart.ProductColor = product.SelectedColors[0];
                    if (string.IsNullOrEmpty(cart.ProductSize)) cart.ProductSize = product.SelectedSizes[0];
                    newOrder.Carts.Add(new CartVM
                    {
                        ProductId = product.id,
                        seller_id = product.seller_id,
                        ProductName = product.name,
                        price = product.discount_price ?? product.price,
                        originalPrice = product.price,
                        CategoryName = product.category?.name ?? "Unknown Category",
                        Quantity = cart.Quantity,
                        ProductColor = cart.ProductColor,
                        ProductSize = cart.ProductSize,
                        imageUrl = cart.imageUrl//uof.ProductRepository.GetProduct_Images(product.id).FirstOrDefault()?.image_url,
                    });
                }
            }
            else if (Request.Query.ContainsKey("productId") && Request.Query.ContainsKey("quantity"))
            {
                //Coming from buy now button with single product
                var productId = Request.Query["productId"].ToString();
                var quantity = int.Parse(Request.Query["quantity"]);
                var product = uof.ProductRepository.getById(productId);
                string productColor = Request.Query["color"].ToString();
                if(string.IsNullOrEmpty(productColor)) productColor = product.SelectedColors[0];
                string productSize = Request.Query["size"].ToString();
                if (string.IsNullOrEmpty(productSize)) productSize = product.SelectedSizes[0];


                if (product == null)
                {
                    TempData["error"] = "Product not found.";
                    return RedirectToAction("Index", "Landing");
                }
                if (quantity <= 0)
                {
                    TempData["error"] = "Quantity must be greater than zero.";
                    return RedirectToAction("Details", new { id = productId });
                }
                newOrder.Carts.Add(new CartVM
                {
                    ProductId = product.id,
                    seller_id = product.seller_id,
                    ProductName = product.name,
                    price = product.discount_price ?? product.price,
                    originalPrice = product.price,
                    CategoryName = product.category?.name ?? "Unknown Category",
                    Quantity = quantity,
                    ProductColor=productColor,
                    ProductSize=productSize,
                    imageUrl = uof.ProductRepository.GetProduct_Images(product.id).FirstOrDefault()?.image_url,

                });
            }
            else
            {
                TempData["error"] = "No products selected for checkout.";
                return RedirectToAction("Index", "Landing");
            }


            return View(newOrder);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ActionName("CheckOut")]
        public async Task<IActionResult> SubmitCheckOut(CheckOutVM model)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "You must be logged in to checkout.";
                return RedirectToAction("Login", "Account");
            }
            if (uof.UserRepository.getById(userId).PhoneNumber == null) 
            {
                uof.UserRepository.getById(userId).PhoneNumber = model.UserPhone;
                uof.UserRepository.Update(uof.UserRepository.getById(userId));
                uof.save();
            }

            decimal totalAmount = model.TotalPrice + model.ShippingTax;  //total price after tax

            //create new order
            var order = new order
            {
                id = Guid.NewGuid().ToString(),
                buyer_id = userId,
                shipping_address = model.shipping_address,
                billing_address = model.shipping_address,
                payment_method = model.payment_method,
                total_amount = totalAmount,
                order_date = DateTime.Now,
                estimated_delivery_date =DateOnly.FromDateTime(DateTime.Now).AddDays(2), //assuming delivery in 7 days
                status = "Pending",
            };

            uof.OrderRepo.add(order);
            uof.save();
            //add order history 
            var newOrderHistory = new order_history
            {
                id = Guid.NewGuid().ToString(),
                order_id = order.id,
                status = "Pending",
                notes = "Order has been created",
                changed_at = DateTime.Now,
                changed_by = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            uof.OrderRepo.AddOrderHistory(newOrderHistory);

            //create order items for each product in the cart
            foreach (var cart in model.Carts)
            {
                var orderItem = new order_item
                {
                    id = Guid.NewGuid().ToString(),
                    order_id = order.id,
                    seller_id = cart.seller_id,
                    product_id = cart.ProductId,
                    quantity = cart.Quantity,
                    Color=cart.ProductColor,
                    Size=cart.ProductSize,
                    discount_applied = cart.originalPrice - cart.price, //price is originalPrice or DiscountPrice
                    unit_price = cart.originalPrice ?? 0,
                    status="Pending",

                };
                uof.OrderRepo.addOrderItem(orderItem);

                //update product stock
                var product = uof.ProductRepository.getById(cart.ProductId);

                if (product != null)
                {
                    product.stock_quantity -= cart.Quantity;
                    uof.ProductRepository.Update(product);
                }
            }
      


            // 🔔 Get unique sellers from the ordered products
            //var sellerIds= model.Carts
            //    .Select(c => c.seller_id)
            //    .Distinct()
            //    .ToList();
            
            //    foreach (var sellerId in sellerIds)
            //    {
            //        string message = $"New order #{order.id} contains one or more of your products.";

            //        //store the notification in the database 
            //        var notification = new notification
            //        {
            //            Id = Guid.NewGuid().ToString(),
            //            RecipientId = sellerId,
            //            Title = "New Order Notification",
            //            Message = message,
            //            Type = "Order",
            //            IsRead = false,
            //            IsDeleted = false,
            //            CreatedAt = DateTime.Now,
            //            OrderId = order.id,

            //        };

            //        // Notify each seller about the new order
            //        await hub.Clients.User(sellerId).SendAsync("ReceiveNotification", message);
            //    }

            
            uof.save();
            //handle stripe payment
            if (model.payment_method == "card")
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = model.Carts.Select(c => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(c.price * 100), // Stripe expects amount in cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = c.ProductName,
                                //Images = new List<string> { c.imageUrl }
                            }
                        },
                        Quantity = c.Quantity
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = Url.Action("orderConfirmation", "Product", new {  orderId = order.id }, protocol: Request.Scheme),
                    CancelUrl = Url.Action("Index", "Cart", new { area = "Customer" }, protocol: Request.Scheme),

                };
                var service = new SessionService();
                Session session = service.Create(options);
                order.payment_status = session.Id;          // Save session ID if needed for future validation
                uof.OrderRepo.Update(order);
                uof.save();

                return Redirect(session.Url);
            }

            TempData["success"] = $"Order placed successfully with ID: {order.id}!";
            return RedirectToAction("Index","Landing");
        }

        public IActionResult orderConfirmation(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                TempData["error"] = "Invalid order ID.";
                return RedirectToAction("Index", "Landing");
            }
            var order = uof.OrderRepo.getById(orderId);
            return View(order);
        }
    }
}