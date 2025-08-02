using Final_project.Models;

namespace Final_project.Repository.CartRepository
{
    public interface ICartItemRepository : IRepository<cart_item>
    {
        List<cart_item> GetCartItemsByCartId(string cart_id);
        public IQueryable<cart_item> GetCartItemsQuery();
        public List<cart_item> GetCartItemsByUserId(string userId);
        void Remove(cart_item entity);
    }
}
