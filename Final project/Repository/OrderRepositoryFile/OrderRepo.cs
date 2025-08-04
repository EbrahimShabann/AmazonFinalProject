using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.OrderRepositoryFile
{
    public class OrderRepo : IOrderRepo
    {
        private readonly AmazonDBContext db;
        public OrderRepo(AmazonDBContext db)
        {
            this.db = db;
        }

        public void add(order entity)
        {
            db.orders.Add(entity);
        }

        public void AddOrderHistory(order_history entity)
        {
            db.order_histories.Add(entity);
        }

        public void addOrderItem(order_item entity)
        {
            db.order_items.Add(entity);
        }

        public void AddReturnOrder(ordersReverted entity)
        {
            db.Orders_Reverted.Add(entity);
        }

        public List<order> getAll()
        {
            return db.orders.Where(o => o.is_deleted != true).ToList();
        }

        public order getById(string id)
        {
            return db.orders.SingleOrDefault(o => o.id == id && !o.is_deleted);
        }

        public order_history GetOrderHistoryByOrderId(string orderId)
        {
            return db.order_histories.SingleOrDefault(oh => oh.order_id == orderId);
        }

        public order_item GetOrderItemById(string id)
        {
            return db.order_items.SingleOrDefault(oi => oi.id == id);
        }

        public List<order_item> GetOrderItemsOfOrder(string orderId)
        {
            return db.order_items
                 .Where(oi => oi.order_id == orderId)
                 .ToList();
        }

        public void Update(order entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public void UpdateOrderHistory(order_history entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        // Additional methods for better querying support

        /// <summary>
        /// Get orders with buyer information included
        /// </summary>
        public List<order> GetAllWithBuyer()
        {
            return db.orders
                .Include(o => o.Buyer)
                .Where(o => o.is_deleted != true)
                .ToList();
        }

        /// <summary>
        /// Get orders by buyer ID
        /// </summary>
        public List<order> GetOrdersByBuyerId(string buyerId)
        {
            return db.orders
                .Where(o => o.buyer_id == buyerId && o.is_deleted != true)
                .OrderByDescending(o => o.order_date)
                .ToList();
        }

        /// <summary>
        /// Get order with all related data
        /// </summary>
        public order GetOrderWithDetails(string orderId)
        {
            return db.orders
                .Include(o => o.Buyer)
                .Include(o => o.OrderItems)
                .SingleOrDefault(o => o.id == orderId && !o.is_deleted);
        }

        /// <summary>
        /// Get recent orders for a specific buyer
        /// </summary>
        public List<order> GetRecentOrdersByBuyer(string buyerId, int count = 5)
        {
            return db.orders
                .Where(o => o.buyer_id == buyerId && o.is_deleted != true)
                .OrderByDescending(o => o.order_date)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Get orders with specific status
        /// </summary>
        public List<order> GetOrdersByStatus(string status)
        {
            return db.orders
                .Where(o => o.status == status && o.is_deleted != true)
                .ToList();
        }

        /// <summary>
        /// Get orders within date range
        /// </summary>
        public List<order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return db.orders
                .Where(o => o.order_date >= startDate &&
                           o.order_date <= endDate &&
                           o.is_deleted != true)
                .OrderByDescending(o => o.order_date)
                .ToList();
        }
    }
}