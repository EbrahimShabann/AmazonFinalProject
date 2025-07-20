using Final_project.Models;
using Final_project.Repository.Product;
using Final_project.ViewModel.Customer;

namespace Final_project.Repository.ProductRepositoryFile
{
    public class ProductRepository : IProductRepository
    {
        private readonly AmazonDBContext db;

        public ProductRepository(AmazonDBContext db)
        {
            this.db = db;
        }
        public void add(product entity)
        {
            db.products.Add(entity);
        }
        //SoftDelete
        public void delete(product entity)
        {
            var product = getById(entity.id);
            if (product != null)
            {
                product.is_deleted = true;
                Update(product);
            }
        }
        //get all exipt the deleted ones
        public List<product> getAll()
        {
            return db.Set<product>().Where(e => e.is_deleted != true).ToList();
        }
        //get product and it's not deleted
        public product getById(string id)
        {
            return db.Set<product>()
                  .Where(e => e.is_deleted != true)
                  .FirstOrDefault(e => e.id == id);
        }

        public List<ProductsVM> getProductsWithImagesAndRating()
        {
            var products = from p in db.products
                           where p.is_deleted != true
                           join img in db.product_images on p.id equals img.product_id
                           join r in db.product_reviews on p.id equals r.product_id
                            where img.is_primary == true
                            select new ProductsVM
                           {
                               id = p.id,
                               name = p.name,
                               price = p.price,
                               discount_price=p.discount_price,
                               Brand = p.Brand,
                               approved_by = p.approved_by,
                               created_at = p.created_at,
                               category_id = p.category_id,
                               Category = p.Category,
                               seller_id = p.seller_id,
                               Seller = p.Seller,
                               stock_quantity = p.stock_quantity,
                               image_url = img.image_url,
                               rating = r.rating
                            };
            return products.ToList();
        }

        public void Update(product entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
        
    }
}
