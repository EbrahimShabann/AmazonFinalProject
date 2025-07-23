using Final_project.Models;

namespace Final_project.Repository.CartRepository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        AmazonDBContext context;
        public ShoppingCartRepository(AmazonDBContext _context)
        {
            context = _context;
        }
        public void add(shopping_cart entity)
        {
            context.shopping_carts.Add(entity);
        }

        public shopping_cart GetShoppingCartByUserId(string user_id)
        {
            return context.shopping_carts.FirstOrDefault(sc => sc.user_id == user_id);
        }

        public shopping_cart getById(string id)
        {
            return context.shopping_carts.Find(id);
        }

        public void save()
        {
            context.SaveChanges();
        }

        public void Update(shopping_cart entity)
        {
            context.shopping_carts.Update(entity);
        }

        public List<shopping_cart> getAll()
        {
            return context.shopping_carts.ToList();
        }
    }
}
