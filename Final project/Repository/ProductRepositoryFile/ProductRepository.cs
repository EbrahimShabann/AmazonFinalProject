using Final_project.Models;
using Final_project.Repository.Product;
using Final_project.ViewModel.Customer;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.ProductRepositoryFile
{
    public class ProductRepository : IProductRepository
    {
        private readonly AmazonDBContext db;

        public ProductRepository(AmazonDBContext db)
        {
            this.db = db;
        }



        public void DeleteReview(product_review entity)
        {
            //delete review
            db.product_reviews.Remove(entity);

        }
        public void DeleteReviewReply(review_reply entity)
        {
            //delete review reply
            db.review_reply.Remove(entity);

        }

        public void add(product entity)
        {
            db.products.Add(entity);
        }

        public async Task<product> GetAsync(System.Linq.Expressions.Expression<System.Func<product, bool>> filter, params System.Linq.Expressions.Expression<System.Func<product, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }

        public async Task<product> GetByIdAsync(string id)
        {
            return await db.products.FirstOrDefaultAsync(p => p.id == id);
        }

        public async Task<int> GetCountAsync(System.Linq.Expressions.Expression<System.Func<product, bool>> filter = null)
        {
            if (filter != null)
                return await db.products.CountAsync(filter);
            return await db.products.CountAsync();
        }

        public async Task AddAsync(product entity)
        {
            await db.products.AddAsync(entity);
        }

        public void Delete(product entity)
        {
            db.products.Remove(entity);
        }

        public IQueryable<product> GetAll(System.Linq.Expressions.Expression<System.Func<product, bool>> filter = null, params System.Linq.Expressions.Expression<System.Func<product, object>>[] includes)
        {
            IQueryable<product> query = db.products;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }



        public void addReview(product_review entity)
        {
            //add review
            db.product_reviews.Add(entity);
        }

        public void delete(product entity)
        {
            //SoftDelete
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

        public List<product_review> getProductReviews(string productId)
        {
            return
                  db.product_reviews.Where(p => p.product_id == productId).ToList();
        }

        public List<ProductsVM> getProductsWithImagesAndRating()
        {
            var products = from p in db.products
                           where p.is_deleted != true
                           join img in db.product_images on p.id equals img.product_id
                           join r in db.product_reviews on p.id equals r.product_id into reviews
                           where img.is_primary == true
                           select new ProductsVM
                           {
                               id = p.id,
                               name = p.name,
                               price = p.price,
                               discount_price = p.discount_price,
                               description = p.description,
                               SelectedColors = p.SelectedColors,
                               SelectedSizes = p.SelectedSizes,
                               Brand = p.Brand,
                               approved_by = p.approved_by,
                               created_at = p.created_at,
                               category_id = p.category_id,
                               Category = p.category,
                               seller_id = p.seller_id,
                               Seller = p.Seller,
                               stock_quantity = p.stock_quantity,
                               image_url = img.image_url,
                               rating = reviews.Any() ? (int)reviews.Average(r => r.rating) : 0,
                               ReviewsCount = reviews.Count(),
                           };
            return products.ToList();
        }

        public List<product_image> GetProduct_Images(string productId)
        {
            return db.product_images.Where(i => i.product_id == productId).ToList();
        }

        public void Update(product entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public List<product_review> getAllReviews()
        {
            return db.product_reviews.ToList();
        }


        public void DeleteReview(product_review entity)
        {
            //delete review
            db.product_reviews.Remove(entity);

        }

    }
}