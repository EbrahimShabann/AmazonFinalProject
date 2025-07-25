using Final_project.Models;

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
            return db.orders.Where(o => o.is_deleted!= true).ToList();
        }

        public order getById(string id)
        {
           return db.orders.SingleOrDefault(o => o.id == id && !o.is_deleted);
        }

        public order_history GetOrderHistoryByOrderId(string orderId)
        {
            return db.order_histories.SingleOrDefault(oh=>oh.order_id == orderId );
        }

        public order_item GetOrderItemById(string id)
        {
            return db.order_items.SingleOrDefault(oi => oi.id == id );
        }

        public List<order_item> GetOrderItemsOfOrder(string orderId)
        {
           return db.order_items
                .Where(oi => oi.order_id == orderId )
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
    }
}
