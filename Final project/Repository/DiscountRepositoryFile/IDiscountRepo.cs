using Final_project.Models;

namespace Final_project.Repository.DiscountRepositoryFile
{
    public interface IDiscountRepo:IRepository<discount>
    {
        public void delete(discount entity);
    }
}
