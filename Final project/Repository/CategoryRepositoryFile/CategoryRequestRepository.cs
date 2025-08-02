
using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CategoryRepositoryFile
{
    public class CategoryRequestRepository : ICategoryRequestRepository
    {
        private readonly AmazonDBContext db;

        public CategoryRequestRepository(AmazonDBContext db)
        {
            this.db = db;
        }
        public void add(CategoryRequest entity) => db.CategoryRequest.Add(entity);

        public List<CategoryRequest> getAll() => db.CategoryRequest.Where(cr => cr.isDeleted == false).ToList();

        public async Task<List<CategoryRequest>> GetAll() => await db.CategoryRequest.Where(cr => cr.isDeleted == false).ToListAsync();

        public CategoryRequest getById(string id) => db.CategoryRequest.Where(cr => cr.isDeleted == false).FirstOrDefault(cr => cr.requredId == id);

        public void HardDelete(string requestId)
        {
            var request=getById(requestId);
            db.CategoryRequest.Remove(request);
        }

        public void SoftDelete(string requestId)
        {
            var request=getById(requestId);
            request.isDeleted = true;
            Update(request);
            db.SaveChanges();

        }

        public void Update(CategoryRequest entity) => db.CategoryRequest.Update(entity);

        // In your repository
        public async Task<bool> HasPendingCategoryAsync(string categoryName)
        {
            return await db.CategoryRequest
                .AnyAsync(r => r.CategoryName.ToLower() == categoryName.ToLower()
                           && r.Status == "pending"
                           && r.isDeleted == false);
        }

        public async Task InsertAsync(CategoryRequest categoryRequest)=> await db.CategoryRequest.AddAsync(categoryRequest);
    }
}
