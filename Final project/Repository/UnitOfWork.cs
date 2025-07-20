using Final_project.Models;
using Final_project.Repository.CustomerServiceRepoFile.ChatMessage;
using Final_project.Repository.CustomerServiceRepoFile.ChatSession;
using Final_project.Repository.CustomerServiceRepoFile.SupportTicket;
using Final_project.Repository.CustomerServiceRepoFile.TicketHistory;
using Final_project.Repository.CustomerServiceRepoFile.TicketMessage;
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

        // Customer Service repositories
        private ISupportTicketRepo _supportTicketRepo;
        private ITicketMessageRepo _ticketMessageRepo;
        private ITicketHistoryRepo _ticketHistoryRepo;
        private IChatSessionRepo _chatSessionRepo;
        private IChatMessageRepo _chatMessageRepo;
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

        // Customer Service repositories
        public ISupportTicketRepo SupportTicketRepo
        {
            get
            {
                if (_supportTicketRepo == null)
                    _supportTicketRepo = new SupportTicketRepo(db);
                return _supportTicketRepo;
            }
        }

        public ITicketMessageRepo TicketMessageRepo
        {
            get
            {
                if (_ticketMessageRepo == null)
                    _ticketMessageRepo = new TicketMessageRepo(db);
                return _ticketMessageRepo;
            }
        }

        public ITicketHistoryRepo TicketHistoryRepo
        {
            get
            {
                if (_ticketHistoryRepo == null)
                    _ticketHistoryRepo = new TicketHistoryRepo(db);
                return _ticketHistoryRepo;
            }
        }

        public IChatSessionRepo ChatSessionRepo
        {
            get
            {
                if (_chatSessionRepo == null)
                    _chatSessionRepo = new ChatSessionRepo(db);
                return _chatSessionRepo;
            }
        }

        public IChatMessageRepo ChatMessageRepo
        {
            get
            {
                if (_chatMessageRepo == null)
                    _chatMessageRepo = new ChatMessageRepo(db);
                return _chatMessageRepo;
            }
        }
        public void save()
        {
            db.SaveChanges();
        }

    }
}
