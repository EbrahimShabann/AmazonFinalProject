using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository.Product
{
    public interface IProductRepository : IRepository<product>
    {
        public IQueryable<product> GetAll(Expression<Func<product, bool>> filter = null, params Expression<Func<product, object>>[] includes);
        public Task<product> GetAsync(Expression<Func<product, bool>> filter, params Expression<Func<product, object>>[] includes);
        public Task<product> GetByIdAsync(string id);
        public Task<int> GetCountAsync(Expression<Func<product, bool>> filter = null);
        public Task AddAsync(product entity);
        public void Delete(product entity);
    }
}
