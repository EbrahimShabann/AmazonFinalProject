using Final_project.Models;

namespace Final_project.Repository.SavedCartRepositoryFile
{
    public interface ISavedCartItemRepository : IRepository<saved_cart_item>
    {
        List<saved_cart_item> GetItemsBySavedCartId(string savedCartId);
        void Remove(saved_cart_item entity);
    }
}