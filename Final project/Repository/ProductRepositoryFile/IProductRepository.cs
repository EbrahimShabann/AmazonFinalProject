using Final_project.Models;
using Final_project.ViewModel.Customer;


namespace Final_project.Repository.Product
{
    public interface IProductRepository : IRepository<product>
    {
         void delete(product entity);
         List<ProductsVM> getProductsWithImagesAndRating();
        List<product_image> GetProduct_Images(string productId);
        List<product_review> getProductReviews(string productId);
        void addReview(product_review review);
    }
}
