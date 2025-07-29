using Final_project.Models;
using Final_project.Repository;
using Final_project.Repository.CartRepository;
using Final_project.ViewModel.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Final_project.Controllers.Wishlist
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public WishlistController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlist = unitOfWork.WishlistRepository.GetWishlistByUserId(userId);

            var items = wishlist != null ? unitOfWork.WishlistItemRepository.GetItemsByWishlistId(wishlist.id) : new List<wishlist_item>();

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
            string user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlist = unitOfWork.WishlistRepository.GetWishlistByUserId(user_id);
            if (wishlist == null)
            {
                wishlist = new wishlist()
                {
                    id = Guid.NewGuid().ToString(),
                    user_id = user_id,
                    created_at = DateTime.UtcNow
                };
                unitOfWork.WishlistRepository.add(wishlist);
                unitOfWork.save();
            }

            var wishlistItem = unitOfWork.WishlistItemRepository.GetByProductId(wishlist.id, productId);

            if (wishlistItem == null)
            {
                var item = new wishlist_item()
                {
                    id = Guid.NewGuid().ToString(),
                    product_id = productId,
                    wishlist_id = wishlist.id,
                    added_at = DateTime.UtcNow
                };
                unitOfWork.WishlistItemRepository.add(item);
                unitOfWork.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(string id)
        {
            wishlist_item item = unitOfWork.WishlistItemRepository.getById(id);
            if (item != null)
            {
                unitOfWork.WishlistItemRepository.Remove(item);
                unitOfWork.save();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MoveToCart(string id)
        {
            string user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            wishlist_item item = unitOfWork.WishlistItemRepository.getById(id);

            if (item == null)
                return RedirectToAction("Index");

            shopping_cart cart = unitOfWork.ShoppingCartRepository.GetShoppingCartByUserId(user_id);

            if (cart == null)
            {
                cart = new shopping_cart()
                {
                    user_id = user_id,
                    id = Guid.NewGuid().ToString(),
                    created_at = DateTime.UtcNow,
                    last_updated_at = DateTime.UtcNow
                };
                unitOfWork.ShoppingCartRepository.add(cart);
                unitOfWork.save();
            }

            cart_item citem = unitOfWork.CartItemRepository.GetCartItemsByCartId(cart.id).FirstOrDefault(ci => ci.product_id == item.product_id);

            if (citem != null)
            {
                citem.quantity++;
                unitOfWork.CartItemRepository.Update(citem);
            }
            else
            {
                cart_item new_cart_item = new cart_item()
                {
                    id = Guid.NewGuid().ToString(),
                    product_id = item.product_id,
                    cart_id = cart.id,
                    quantity = 1,
                    added_at = DateTime.UtcNow
                };
                unitOfWork.CartItemRepository.add(new_cart_item);
            }

            unitOfWork.save();

            unitOfWork.WishlistItemRepository.Remove(item);
            unitOfWork.save();

            return RedirectToAction("Index");
        }
    }
}