using Final_project.Models;

namespace Final_project.Repository.SavedCartRepositoryFile
{
    public interface ISavedCartRepository : IRepository<saved_cart>
    {
        List<saved_cart> GetSavedCartsByUserId(string userId);
    }
}