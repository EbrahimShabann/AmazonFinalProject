using System.Linq.Expressions;
using Final_project.Models;
using Microsoft.EntityFrameworkCore;
namespace Final_project.Repository
{
    public interface IProductDiscountRepository : IRepository<product_discount> {

        public IQueryable<product_discount> GetAll(Expression<Func<product_discount, bool>> filter = null, params Expression<Func<product_discount, object>>[] includes);
        public Task<product_discount> GetAsync(Expression<Func<product_discount, bool>> filter, params Expression<Func<product_discount, object>>[] includes);
        public Task<product_discount> GetByIdAsync(string id);
        public Task<int> GetCountAsync(Expression<Func<product_discount, bool>> filter = null);
        public Task AddAsync(product_discount entity) ;
        public void Delete(product_discount entity);

    }
} 