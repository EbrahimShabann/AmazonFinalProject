using Final_project.Models;

namespace Final_project.Repository.CategoryRepositoryFile
{
    public interface ICategoryRequestRepository:IRepository<CategoryRequest>
    {
        public void SoftDelete(string requestId);
        public void HardDelete(string requestId);

    }
}
