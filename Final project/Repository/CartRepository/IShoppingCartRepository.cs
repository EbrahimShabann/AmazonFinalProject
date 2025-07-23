using Final_project.Models;

namespace Final_project.Repository.CartRepository
{
    public interface IShoppingCartRepository : IRepository<shopping_cart>
    {
        shopping_cart GetShoppingCartByUserId(string user_id);
        void save();
    }
}
