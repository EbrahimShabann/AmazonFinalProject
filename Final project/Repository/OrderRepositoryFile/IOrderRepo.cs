using Final_project.Models;

namespace Final_project.Repository.OrderRepositoryFile
{
    public interface IOrderRepo:IRepository<Order>
    {
        public void delete(Order entity);
    }
}
