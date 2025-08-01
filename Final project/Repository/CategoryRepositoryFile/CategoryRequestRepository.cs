
using Final_project.Models;

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



  

    }
}
