using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.WishlistRepository
{
    public class WishlistItemRepository : IWishlistItemRepository
    {
        AmazonDBContext context;
        public WishlistItemRepository(AmazonDBContext _context)
        {
            context = _context;
        }
        public void add(wishlist_item entity)
        {
            context.Add(entity);
        }

        public List<wishlist_item> getAll()
        {
            return context.wishlist_items.Include(i => i.Product).ToList();
        }

        public wishlist_item getById(string id)
        {
            return context.wishlist_items.Include(i => i.Product).FirstOrDefault(i => i.id == id);
        }

        public wishlist_item GetByProductId(string wishlistId, string productId)
        {
            return context.wishlist_items.Include(i => i.Product).FirstOrDefault(i => i.wishlist_id == wishlistId && i.product_id == productId);
        }

        public List<wishlist_item> GetItemsByWishlistId(string wishlistId)
        {
            return context.wishlist_items.Include(i => i.Product).Where(i => i.wishlist_id == wishlistId).ToList();
        }

        public void Remove(wishlist_item item)
        {
            context.wishlist_items.Remove(item);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(wishlist_item entity)
        {
            context.wishlist_items.Update(entity);
        }
    }
}
