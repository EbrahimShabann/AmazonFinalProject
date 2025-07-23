using Final_project.Models;
using Final_project.Repository.CartRepository;
using Final_project.Repository.WishlistRepository;
using Final_project.ViewModel.Wishlist;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WishlistController : Controller
    {
        private readonly IWishlistRepository wishlistRepo;
        private readonly IWishlistItemRepository wishlistItemRepo;
        private readonly IShoppingCartRepository cartRepo;
        private readonly ICartItemRepository cartItemRepo;

        public WishlistController(IWishlistRepository _wishlistRepo, IWishlistItemRepository _wishlistItemRepo, IShoppingCartRepository _cartRepo, ICartItemRepository _cartItemRepo)
        {
            wishlistRepo = _wishlistRepo;
            wishlistItemRepo = _wishlistItemRepo;
            cartRepo = _cartRepo;
            cartItemRepo = _cartItemRepo;
        }

        public IActionResult Index()
        {
            string userId = "c1";
            var wishlist = wishlistRepo.GetWishlistByUserId(userId);

            var items = wishlist != null ? wishlistItemRepo.GetItemsByWishlistId(wishlist.id) : new List<wishlist_item>();

            var itemViewModel = items.Select(i => new WishlistItemViewModel
            {
                ItemId = i.id,
                ProductId = i.product_id,
                ProductName = i.Product?.name ?? "Unknown",
                Price = i.Product?.discount_price ?? i.Product.price ?? 0,
                InStock = i.Product?.stock_quantity > 0,
                ImageUrl = "/images/m.png"
            }).ToList();
            return View(itemViewModel);
        }

        public IActionResult AddToWishlist(string productId)
        {
            string user_id = "c1";

            var wishlist = wishlistRepo.GetWishlistByUserId(user_id);
            if (wishlist == null)
            {
                wishlist = new wishlist()
                {
                    id = Guid.NewGuid().ToString(),
                    user_id = user_id,
                    created_at = DateTime.UtcNow
                };
                wishlistRepo.add(wishlist);
                wishlistRepo.Save();
            }

            var wishlistItem = wishlistItemRepo.GetByProductId(wishlist.id, productId);

            if (wishlistItem == null)
            {
                var item = new wishlist_item()
                {
                    id = Guid.NewGuid().ToString(),
                    product_id = productId,
                    wishlist_id = wishlist.id,
                    added_at = DateTime.UtcNow
                };
                wishlistItemRepo.add(item);
                wishlistItemRepo.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromWishlist(string id) {
            wishlist_item item = wishlistItemRepo.getById(id);
            if (item != null)
            {
                wishlistItemRepo.Remove(item);
                wishlistItemRepo.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult MoveToCart(string id)
        {
            string user_id = "c1";
            wishlist_item item = wishlistItemRepo.getById(id);

            if (item == null)
                return RedirectToAction("Index");

            shopping_cart cart = cartRepo.GetShoppingCartByUserId(user_id);

            if (cart == null)
            {
                cart = new shopping_cart()
                {
                    user_id = user_id,
                    id = Guid.NewGuid().ToString(),
                    created_at = DateTime.UtcNow,
                    last_updated_at = DateTime.UtcNow
                };
                cartRepo.add(cart);
                cartRepo.save();
            }

            cart_item citem = cartItemRepo.GetCartItemsByCartId(cart.id).FirstOrDefault(ci => ci.product_id == item.product_id);

            if (citem != null)
            {
                citem.quantity++;
                cartItemRepo.Update(citem);
            }else
            {
                cart_item new_cart_item = new cart_item()
                {
                    id = Guid.NewGuid().ToString(),
                    product_id = item.product_id,
                    cart_id = cart.id,
                    quantity = 1,
                    added_at = DateTime.UtcNow
                };
            }

            cartItemRepo.save();

            wishlistItemRepo.Remove(item);
            wishlistItemRepo.Save();

            return RedirectToAction("Index");
        }
    }
}
