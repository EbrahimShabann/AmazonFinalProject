using Final_project.Models;

namespace Final_project.Repository.OrderRepositoryFile
{
    public interface IOrderRepo:IRepository<order>
    {
        public void delete(order entity);
    }
}
