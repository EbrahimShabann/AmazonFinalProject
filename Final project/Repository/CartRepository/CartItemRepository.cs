using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CartRepository
{
    public class CartItemRepository : ICartItemRepository
    {
        AmazonDBContext context;
        public CartItemRepository(AmazonDBContext _context)
        {
            context = _context;
        }

        public void add(cart_item entity)
        {
            context.cart_items.Add(entity);
        }

        public List<cart_item> GetCartItemsByCartId(string cart_id)
        {
            return context.cart_items
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .Where(ci => ci.cart_id == cart_id).ToList();
        }

        public cart_item getById(string id)
        {
            return context.cart_items
                .Include(c => c.Product)
                .Include(c => c.Cart)
                .FirstOrDefault(c => c.id == id);
        }

        public void save()
        {
            context.SaveChanges();
        }

        public void Update(cart_item entity)
        {
            context.cart_items.Update(entity);
        }

        public void Remove(cart_item entity)
        {
            context.cart_items.Remove(entity);
        }

        public List<cart_item> getAll()
        {
            return context.cart_items
                .Include(c => c.Product)
                .Include(c => c.Cart)
                .ToList();
        }

        // Additional method for getting cart items with specific includes
        public IQueryable<cart_item> GetCartItemsQuery()
        {
            return context.cart_items
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart);
        }

        // Method specifically for chatbot controller to get cart items with user filtering
        public List<cart_item> GetCartItemsByUserId(string userId)
        {
            return context.cart_items
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.user_id == userId)
                .ToList();
        }
    }
}