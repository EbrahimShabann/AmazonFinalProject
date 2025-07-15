using Final_project.Models;
using Final_project.Models;

namespace Final_project.Repository.Product
{
    public interface IProductRepository : IRepository<product>
    {
        public void delete(product entity);

    }
}
