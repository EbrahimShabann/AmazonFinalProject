using Final_project.Models;
using Final_project.Repository.AccountRepositoryFile;
using Final_project.Repository.CartRepository;
using Final_project.Repository.CategoryFile;
using Final_project.Repository.LandingPageFile;
using Final_project.Repository.MessagesRepositoryFile;
using Final_project.Repository.NewFolder;
using Final_project.Repository.OrderRepositoryFile;
using Final_project.Repository.Product;
using Final_project.Repository.ProductRepositoryFile;
using Final_project.Repository.SavedCartRepositoryFile;
using Final_project.Repository.WishlistRepository;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository
{
    public class UnitOfWork
    {
        private readonly AmazonDBContext db;
        private IProductRepository _productRepository;
        private ILandingPageRepository _landingPageReposotory;
        private ICategoryRepository _categoryRepository;
        private IAccountRepository _accountRepository;
        private ICartItemRepository _cartItemRepository;
        private IShoppingCartRepository _shoppingCartRepository;
        private IDiscountRepository _discountRepository;
        private IOrderItemRepository _orderItemRepository;
        private IOrderRepository _orderRepository;
        private IProductDiscountRepository _productDiscountRepository;
        private IProductImageRepository _productImageRepository;
        private IUserRepository _userRepository;
        private IOrderRepo _orderRepo;
        private IMessagesRepo _messageRepo;
        private IWishlistItemRepository _wishlistItemRepository;
        private IWishlistRepository _wishlistRepository;
        private ISavedCartRepository _savedCartRepository;
        private ISavedCartItemRepository _savedCartItemRepository;




        public UnitOfWork(AmazonDBContext db)
        {
            this.db = db;
        }

        public ILandingPageRepository LandingPageReposotory
        {
            get
            {
                if (_landingPageReposotory == null)
                    _landingPageReposotory = new LandingPageRepository(db);
                return _landingPageReposotory;
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                    _categoryRepository = new CategoryRepository(db);
                return _categoryRepository;
            }
        }
        public IAccountRepository AccountRepository
        {
            get
            {
                if (_accountRepository == null)
                    _accountRepository = new AccountRepository(db);
                return _accountRepository;
            }
        }
        public ICartItemRepository CartItemRepository
        {
            get
            {
                if (_cartItemRepository == null)
                    _cartItemRepository = new CartItemRepository(db);
                return _cartItemRepository;
            }
        }
        public IShoppingCartRepository ShoppingCartRepository
        {
            get
            {
                if (_shoppingCartRepository == null)
                    _shoppingCartRepository = new ShoppingCartRepository(db);
                return _shoppingCartRepository;
            }
        }
        public IDiscountRepository DiscountRepository
        {
            get
            {
                if (_discountRepository == null)
                    _discountRepository = new DiscountRepository(db);
                return _discountRepository;
            }
        }
        public IOrderItemRepository OrderItemRepository
        {
            get
            {
                if (_orderItemRepository == null)
                    _orderItemRepository = new OrderItemRepository(db);
                return _orderItemRepository;
            }
        }
        public IOrderRepository OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new OrderRepository(db);
                return _orderRepository;
            }
        }

        public IProductDiscountRepository ProductDiscountRepository
        {
            get
            {
                if (_productDiscountRepository == null)
                    _productDiscountRepository = new ProductDiscountRepository(db);
                return _productDiscountRepository;
            }
        }
        public IProductImageRepository ProductImageRepository
        {
            get
            {
                if (_productImageRepository == null)
                    _productImageRepository = new ProductImageRepository(db);
                return _productImageRepository;
            }
        }
        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(db);
                return _userRepository;
            }
        }
        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new ProductRepository(db);
                return _productRepository;
            }
        }
        public IOrderRepo OrderRepo
        {
            get
            {
                if (_orderRepo == null)
                    _orderRepo = new OrderRepo(db);
                return _orderRepo;
            }
        }
        public IMessagesRepo MessageRepo
        {
            get
            {
                if (_messageRepo == null)
                    _messageRepo = new MessageRepo(db);
                return _messageRepo;
            }
        }

        public IWishlistItemRepository WishlistItemRepository
        {
            get
            {
                if (_wishlistItemRepository == null)
                    _wishlistItemRepository = new WishlistItemRepository(db);
                return _wishlistItemRepository;
            }
        }
        public IWishlistRepository WishlistRepository
        {
            get
            {
                if (_wishlistRepository == null)
                    _wishlistRepository = new WishlistRepository.WishlistRepository(db);
                return _wishlistRepository;
            }
        }

        public ISavedCartRepository SavedCartRepository
        {
            get
            {
                if (_savedCartRepository == null)
                    _savedCartRepository = new SavedCartRepository(db);
                return _savedCartRepository;
            }
        }

        public ISavedCartItemRepository SavedCartItemRepository
        {
            get
            {
                if (_savedCartItemRepository == null)
                    _savedCartItemRepository = new SavedCartItemRepository(db);
                return _savedCartItemRepository;
            }
        }


        public void save()
        {
            db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
