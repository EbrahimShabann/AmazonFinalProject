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
    }
}
