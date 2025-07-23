using Final_project.Models;

namespace Final_project.Repository.CartRepository
{
    public interface ICartItemRepository : IRepository<cart_item>
    {
        List<cart_item> GetCartItemsByCartId(string cart_id);
        void Remove(cart_item entity);
    }
}
