using Final_project.Models;
using Final_project.Repository.AccountRepositoryFile;
using Final_project.Repository.CartRepository;
using Final_project.Repository.CategoryFile;
using Final_project.Repository.NewFolder;
using Final_project.Repository.ProductRepositoryFile;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository
{
    public class UnitOfWork
    {
        private readonly AmazonDBContext db;
        //private IProductRepository _productRepository;
        private ILandingPageRepository _landingPageReposotory;
        private ICategoryRepository _categoryRepository;
        private IAccountRepository _accountRepository;
        private ICartItemRepository _cartItemRepository;
        private IShoppingCartRepository _shoppingCartRepository;

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

        public void save()
        {
            db.SaveChanges();
        }

    }
}
