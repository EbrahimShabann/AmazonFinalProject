using System.Linq.Expressions;
using Final_project.Models;
using Microsoft.EntityFrameworkCore;
namespace Final_project.Repository
{
    public interface IProductImageRepository : IRepository<product_image> {
        public IQueryable<product_image> GetAll(Expression<Func<product_image, bool>> filter = null, params Expression<Func<product_image, object>>[] includes);
        public  Task<product_image> GetAsync(Expression<Func<product_image, bool>> filter, params Expression<Func<product_image, object>>[] includes);
        public  Task<product_image> GetByIdAsync(string id);
        public  Task<int> GetCountAsync(Expression<Func<product_image, bool>> filter = null);
        public void Delete(product_image entity);
    }
} 