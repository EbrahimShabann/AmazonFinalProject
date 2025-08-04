using Final_project.Models;

namespace Final_project.Repository.OrderRepositoryFile
{
    public interface IOrderRepo:IRepository<order>
    {
       void addOrderItem(order_item entity);
        List<order_item> GetOrderItemsOfOrder(string orderId);
        order_item GetOrderItemById(string id);
        order_history GetOrderHistoryByOrderId(string orderId);
        void UpdateOrderHistory(order_history entity);
        void AddOrderHistory(order_history entity);

        void AddReturnOrder(ordersReverted entity);


        //for more pefomance
        public List<order> GetAllWithBuyer();
        public List<order> GetOrdersByBuyerId(string buyerId);
        public order GetOrderWithDetails(string orderId);
        public List<order> GetRecentOrdersByBuyer(string buyerId, int count = 5);
        public List<order> GetOrdersByStatus(string status);
        public List<order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);

    }
}
