using Final_project.Models;

namespace Final_project.Repository.SavedCartRepositoryFile
{
    public class SavedCartItemRepository : ISavedCartItemRepository
    {
        AmazonDBContext context;
        public SavedCartItemRepository(AmazonDBContext _context)
        {
            context = _context;
        }
        public void add(saved_cart_item entity)
        {
            context.saved_cart_items.Add(entity);
        }

        public List<saved_cart_item> getAll()
        {
            return context.saved_cart_items.ToList();
        }

        public saved_cart_item getById(string id)
        {
            return context.saved_cart_items.Find(id);
        }

        public List<saved_cart_item> GetItemsBySavedCartId(string savedCartId)
        {
            return context.saved_cart_items.Where(i => i.saved_cart_id == savedCartId).ToList();
        }

        public void Remove(saved_cart_item entity)
        {
            context.saved_cart_items.Remove(entity);
        }

        public void Update(saved_cart_item entity)
        {
            context.saved_cart_items.Update(entity);
        }
    }
}