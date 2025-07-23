using Final_project.Models;
using Final_project.Repository.CartRepository;
using Final_project.ViewModel.Cart;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers.Cart
{
    public class CartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepo;
        private readonly ICartItemRepository _cartItemRepo;

        public CartController(IShoppingCartRepository shoppingCartRepo, ICartItemRepository cartItemRepo)
        {
            _shoppingCartRepo = shoppingCartRepo;
            _cartItemRepo = cartItemRepo;
        }
        public IActionResult Index()
        {
            string userId = "c1";
            var cart = _shoppingCartRepo.GetShoppingCartByUserId(userId);

            if (cart == null)
            {
                ViewBag.Subtotal = 0;
                return View(new List<CartItemViewModel>());
            }

            var items = _cartItemRepo.GetCartItemsByCartId(cart.id);

            var viewModel = items.Select(item => new CartItemViewModel
            {
                CartItemId = item.id,
                ProductId = item.product_id,
                ProductName = item.Product?.name ?? "Unknown",
                ImageUrl = "/images/m.png",
                Quantity = item.quantity ?? 1,
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
            var item = _cartItemRepo.getById(id);
            if (item != null)
            {
                item.quantity++;
                _cartItemRepo.Update(item);
                _cartItemRepo.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Decrease(string id)
        {
            var item = _cartItemRepo.getById(id);
            if (item != null && item.quantity > 1)
            {
                item.quantity--;
                _cartItemRepo.Update(item);
                _cartItemRepo.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var item = _cartItemRepo.getById(id);
            if (item != null)
            {
                _cartItemRepo.Remove(item);
                _cartItemRepo.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToCart(string productId)
        {
            string userId = "c7"; 

            // Get or create cart
            var cart = _shoppingCartRepo.GetShoppingCartByUserId(userId);
            if (cart == null)
            {
                cart = new shopping_cart
                {
                    id = Guid.NewGuid().ToString(),
                    user_id = userId,
                    created_at = DateTime.UtcNow,
                    last_updated_at = DateTime.UtcNow
                };
                _shoppingCartRepo.add(cart);
                _shoppingCartRepo.save();
            }

            // Check if product already exists in cart
            var existingItem = _cartItemRepo.GetCartItemsByCartId(cart.id)
                                .FirstOrDefault(ci => ci.product_id == productId);

            if (existingItem != null)
            {
                existingItem.quantity++;
                _cartItemRepo.Update(existingItem);
            }
            else
            {
                var newItem = new cart_item
                {
                    id = Guid.NewGuid().ToString(),
                    cart_id = cart.id,
                    product_id = productId,
                    quantity = 1,
                    added_at = DateTime.UtcNow
                };
                _cartItemRepo.add(newItem);
            }

            _cartItemRepo.save();

            return RedirectToAction("Index");
        }
    }
}
