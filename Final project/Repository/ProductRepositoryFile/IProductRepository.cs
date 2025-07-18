using Final_project.Models;
using Final_project.ViewModel.Seller;


namespace Final_project.Repository.Product
{
    public interface IProductRepository : IRepository<product>
    {
         void delete(product entity);
         List<ProductsVM> getProductsWithImages();

    }
}
