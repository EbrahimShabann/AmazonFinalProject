using System.Threading.Tasks;
using Final_project.Repository.ProductRepositoryFile;

namespace Final_project.Repository
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IDiscountRepository Discounts { get; }
        ICategoryRepository Categories { get; }
        IUserRepository Users { get; }
        IProductImageRepository ProductImages { get; }
        IOrderItemRepository OrderItems { get; }
        IProductDiscountRepository ProductDiscounts { get; }
        Task<int> SaveAsync();
    }
} 