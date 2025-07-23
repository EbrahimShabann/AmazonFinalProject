using Final_project.Models;
using Final_project.Repository.OrderRepositoryFile;
using Final_project.Repository.Product;
using Final_project.Repository.ProductRepositoryFile;

namespace Final_project.Repository
{
    public class UnitOfWork
    {
        private readonly AmazonDBContext db;
        private IProductRepository _productRepository;
        private IOrderRepo _orderRepo;
       
        public UnitOfWork(AmazonDBContext db)
        {
            this.db = db;
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


        public void save()
        {
            db.SaveChanges();
        }

    }
}
