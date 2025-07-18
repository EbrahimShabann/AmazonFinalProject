using Final_project.Models;
using Final_project.Repository.DiscountRepositoryFile;
using Final_project.Repository.OrderRepositoryFile;
using Final_project.Repository.Product;
using Final_project.Repository.ProductImagesRepositoryFile;
using Final_project.Repository.ProductRepositoryFile;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository
{
    public class UnitOfWork
    {
        private readonly AmazonDBContext db;
        private IProductRepository _productRepository;
        private IOrderRepo _orderRepo;
        private IDiscountRepo _discountRepo;
        private IProductImageRepo _productImageRepo;
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
        public IDiscountRepo DiscountRepo
        {
            get
            {
                if (_discountRepo == null)
                    _discountRepo = new DiscountRepo(db);
                return _discountRepo;
            }
        }


        public IProductImageRepo ProductImageRepo 
        { 
            get
            {
                if (_productImageRepo == null)
                    _productImageRepo = new ProductImageRepo(db);
                return _productImageRepo;
            } 
        }
        public void save()
        {
            db.SaveChanges();
        }

    }
}
