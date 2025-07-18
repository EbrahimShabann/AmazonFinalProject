using Final_project.Models;

namespace Final_project.Repository.ProductImagesRepositoryFile
{
    public interface IProductImageRepo:IRepository<product_image>
    {
        void delete(product_image entity);
        List<product_image> getImagesOfProduct(string productId);
    }
}
