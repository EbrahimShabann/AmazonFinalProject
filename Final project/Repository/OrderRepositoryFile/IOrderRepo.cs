using Final_project.Models;

namespace Final_project.Repository.OrderRepositoryFile
{
    public interface IOrderRepo:IRepository<order>
    {
       void addOrderItem(order_item entity);
    }
}
