using System.Linq.Expressions;
using Final_project.Models;
using Final_project.ViewModel.Customer;

namespace Final_project.Repository.Product
{
    public interface IProductRepository : IRepository<product>
    {
        public IQueryable<product> GetAll(Expression<Func<product, bool>> filter = null, params Expression<Func<product, object>>[] includes);
        public Task<product> GetAsync(Expression<Func<product, bool>> filter, params Expression<Func<product, object>>[] includes);
        public Task<product> GetByIdAsync(string id);
        public Task<int> GetCountAsync(Expression<Func<product, bool>> filter = null);
        public Task AddAsync(product entity);
        public void Delete(product entity);

        public void DeleteReview(product_review entity);
        void delete(product entity);
        List<ProductsVM> getProductsWithImagesAndRating();
        List<product_image> GetProduct_Images(string productId);
        List<product_review> getProductReviews(string productId);
        List<product_review> getAllReviews();
        void addReview(product_review review);
        void DeleteReviewReply(review_reply reply);



        public Task<product_review> GetProductReviewByIdAsync(string productReviewId);
        public void AddReviewReply (review_reply reply);

    }
}