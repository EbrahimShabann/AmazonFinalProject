using System.Linq.Expressions;
using Final_project.Models;
using Final_project.ViewModel.LandingPageViewModels;
using Final_project.ViewModel.NewFolder;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CategoryFile
{
    public interface ICategoryRepository:IRepository<category>
    {
        public List<CategoryViewModel> GetCategoryWithItsChildern();
        public int totalProduct();
        public IQueryable<category> GetAll(Expression<Func<category, bool>> filter = null, params Expression<Func<category, object>>[] includes);
        public Task<category> GetAsync(Expression<Func<category, bool>> filter, params Expression<Func<category, object>>[] includes);
        public  Task<category> GetByIdAsync(string id);
        public Task<int> GetCountAsync(Expression<Func<category, bool>> filter = null);
        public  Task AddAsync(category entity) ;
        public void Delete(category entity);
    }
}
