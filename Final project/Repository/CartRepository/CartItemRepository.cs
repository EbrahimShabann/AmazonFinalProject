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
                //.Include(ci => ci.Product)
                .Where(ci => ci.cart_id == cart_id).ToList();
        }

        public cart_item getById(string id)
        {
            //return context.cart_items.Include(c => c.Product).FirstOrDefault(c => c.id == id);
            return context.cart_items.FirstOrDefault(c => c.id == id);
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
            //return context.cart_items.Include(c => c.Product).ToList();
            return context.cart_items.ToList();
        }


    }
}
