using Final_project.Models;

namespace Final_project.Repository.ProductImagesRepositoryFile
{
    public class ProductImageRepo : IProductImageRepo
    {
        private readonly AmazonDBContext db;

        public ProductImageRepo(AmazonDBContext db)
        {
            this.db = db;
        }
        public void add(product_image entity)
        {
            db.product_images.Add(entity);
        }

        public List<product_image> getAll()
        {
            throw new NotImplementedException();
        }

        public product_image getById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(product_image entity)
        {
            throw new NotImplementedException();
        }
    }
}
