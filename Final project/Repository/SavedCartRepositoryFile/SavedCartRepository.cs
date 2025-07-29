using Final_project.Models;

namespace Final_project.Repository.SavedCartRepositoryFile
{
    public class SavedCartRepository : ISavedCartRepository
    {
        AmazonDBContext context;
        public SavedCartRepository(AmazonDBContext _context)
        {
            context = _context;
        }
        public void add(saved_cart entity)
        {
            context.saved_carts.Add(entity);
        }

        public List<saved_cart> getAll()
        {
            return context.saved_carts.ToList();
        }

        public saved_cart getById(string id)
        {
            return context.saved_carts.Find(id);
        }

        public List<saved_cart> GetSavedCartsByUserId(string userId)
        {
            return context.saved_carts.Where(sc => sc.user_id == userId).OrderByDescending(sc => sc.created_at).ToList();
        }

        public void Update(saved_cart entity)
        {
            context.saved_carts.Update(entity);
        }
    }
}
