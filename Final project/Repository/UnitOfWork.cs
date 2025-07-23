using System.Threading.Tasks;
using Final_project.Models;
using Final_project.Repository.ProductRepositoryFile;

namespace Final_project.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AmazonDBContext _context;
        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IDiscountRepository Discounts { get; }
        public ICategoryRepository Categories { get; }
        public IUserRepository Users { get; }
        public IProductImageRepository ProductImages { get; }
        public IOrderItemRepository OrderItems { get; }
        public IProductDiscountRepository ProductDiscounts { get; }
        // تم حذف ProductReviews, ReviewReplies, ChatSessions, ChatMessages, Notifications بناءً على طلبك

        public UnitOfWork(AmazonDBContext context)
        {
            _context = context;
            Products = new ProductRepositoryFile.ProductRepository(context);
            Orders = new OrderRepository(context);
            Discounts = new DiscountRepository(context);
            Categories = new CategoryRepository(context);
            Users = new UserRepository(context);
            ProductImages = new ProductImageRepository(context);
            OrderItems = new OrderItemRepository(context);
            ProductDiscounts = new ProductDiscountRepository(context);
            // تم حذف ProductReviews, ReviewReplies, ChatSessions, ChatMessages, Notifications بناءً على طلبك
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
