using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.WishlistRepository
{
    public class WishlistRepository : IWishlistRepository
    {
        AmazonDBContext context;

        public WishlistRepository(AmazonDBContext _context)
        {
            context = _context;
        }

        public void add(wishlist entity)
        {
            context.wishlists.Add(entity);
        }

        public List<wishlist> getAll()
        {
            return context.wishlists.ToList();
        }

        public wishlist getById(string id)
        {
            return context.wishlists.Find(id);
        }

        public wishlist GetWishlistByUserId(string userId)
        {
            return context.wishlists.Include(w => w.Items).ThenInclude(i => i.Product).FirstOrDefault(w => w.user_id == userId);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(wishlist entity)
        {
            context.wishlists.Update(entity);
        }
    }
}
