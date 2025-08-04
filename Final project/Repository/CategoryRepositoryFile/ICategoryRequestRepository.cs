using Final_project.Models;

namespace Final_project.Repository.CategoryRepositoryFile
{
    public interface ICategoryRequestRepository:IRepository<CategoryRequest>
    {
        public Task<List<CategoryRequest>> GetAll();
        public void SoftDelete(string requestId);
        public void HardDelete(string requestId);
        public Task<bool> HasPendingCategoryAsync(string categoryName);
        public Task InsertAsync(CategoryRequest categoryRequest);


    }
}
