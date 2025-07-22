using Final_project.Models;
using Final_project.Repository.AccountRepositoryFile;
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


        public void save()
        {
            db.SaveChanges();
        }

    }
}
