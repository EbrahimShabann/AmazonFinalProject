using Final_project.Models;
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

        public void save()
        {
            db.SaveChanges();
        }

    }
}
