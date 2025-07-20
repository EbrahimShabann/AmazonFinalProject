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

        public void delete(product_image entity)
        {
            db.product_images.Remove(entity);
        }

        public List<product_image> getAll()
        {
            return db.product_images.ToList();
        }

        public product_image getById(string id)
        {
            throw new NotImplementedException();
        }

        public List<product_image> getImagesOfProduct(string productId)
        {
            return getAll().Where(img => img.product_id == productId).ToList();
        }

        public void Update(product_image entity)
        {
            throw new NotImplementedException();
        }
    }
}
