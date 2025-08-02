using System.Security.Claims;
using Final_project.Models;
using Final_project.Repository;
using Final_project.Repository.CartRepository;
using Final_project.ViewModel.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers.Cart
{
    public class CartController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public CartController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = unitOfWork.ShoppingCartRepository.GetShoppingCartByUserId(userId);

            if (cart == null)
            {
                ViewBag.Subtotal = 0;
                return View(new List<CartItemViewModel>());
            }

            var items = unitOfWork.CartItemRepository.GetCartItemsByCartId(cart.id);

            var viewModel = items.Select(item => new CartItemViewModel
            {
                CartItemId = item.id,
                ProductId = item.product_id,
                ProductName = item.Product?.name ?? "Unknown",
                ImageUrl = unitOfWork.ProductRepository.GetProduct_Images(item.product_id).SingleOrDefault(i => i.is_primary == true).image_url,
                Quantity = item.quantity ?? 1,
                Color = item.color,
                Size = item.size,
                Price = item.Product?.discount_price ?? item.Product?.price ?? 0,
                InStock = item.Product?.stock_quantity > 0,
                Badge = item.Product?.stock_quantity > 50 ? "#1 Best Seller" : null
            }).ToList();

            ViewBag.Subtotal = viewModel.Sum(i => i.Quantity * i.Price);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Increase(string id)
        {
            var item = unitOfWork.CartItemRepository.getById(id);
            if (item != null)
            {
                item.quantity++;
                unitOfWork.CartItemRepository.Update(item);
                unitOfWork.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Decrease(string id)
        {
            var item = unitOfWork.CartItemRepository.getById(id);
            if (item != null && item.quantity > 1)
            {
                item.quantity--;
                unitOfWork.CartItemRepository.Update(item);
                unitOfWork.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var item = unitOfWork.CartItemRepository.getById(id);
            if (item != null)
            {
                unitOfWork.CartItemRepository.Remove(item);
                unitOfWork.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(string productId, string color, string size)
        {
            var IdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string userId = IdClaim.Value;
            var cart = unitOfWork.ShoppingCartRepository.GetShoppingCartByUserId(userId);
            if (cart == null)
            {
                cart = new shopping_cart
                {
                    id = Guid.NewGuid().ToString(),
                    user_id = userId,
                    created_at = DateTime.UtcNow,
                    last_updated_at = DateTime.UtcNow
                };
                unitOfWork.ShoppingCartRepository.add(cart);
                unitOfWork.save();
            }

            var existingItem = unitOfWork.CartItemRepository.GetCartItemsByCartId(cart.id)
                                .FirstOrDefault(ci => ci.product_id == productId);

            if (existingItem != null)
            {
                existingItem.quantity++;
                unitOfWork.CartItemRepository.Update(existingItem);
            }
            else
            {
                var newItem = new cart_item
                {
                    id = Guid.NewGuid().ToString(),
                    cart_id = cart.id,
                    product_id = productId,
                    quantity = 1,
                    color = color,
                    size = size,
                    added_at = DateTime.UtcNow
                };
                unitOfWork.CartItemRepository.add(newItem);
            }

            unitOfWork.save();
            var val = unitOfWork.LandingPageReposotory.GetCartCount(User.Identity.Name);
            return Json(val);
        }

        [HttpPost]
        public IActionResult SaveCurrentCart(string cartName)
        {
            var IdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string userId = IdClaim.Value;

            var cart = unitOfWork.ShoppingCartRepository.GetShoppingCartByUserId(userId);
            if (cart == null)
                return BadRequest("No cart to save.");

            var cartItems = unitOfWork.CartItemRepository.GetCartItemsByCartId(cart.id);
            if (!cartItems.Any())
                return BadRequest("Cart is empty.");

            var savedCart = new saved_cart
            {
                id = Guid.NewGuid().ToString(),
                user_id = userId,
                name = cartName,
                created_at = DateTime.UtcNow
            };

            unitOfWork.SavedCartRepository.add(savedCart);

            foreach (var item in cartItems)
            {
                var savedItem = new saved_cart_item
                {
                    id = Guid.NewGuid().ToString(),
                    saved_cart_id = savedCart.id,
                    product_id = item.product_id,
                    quantity = item.quantity ?? 1
                };

                unitOfWork.SavedCartItemRepository.add(savedItem);
            }

            unitOfWork.save();
            return RedirectToAction("Index");
        }
    }
}