using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Final_project.Models;
using Microsoft.EntityFrameworkCore;
namespace Final_project.Repository
{
    public interface IOrderItemRepository : IRepository<order_item> {
        public IQueryable<order_item> GetAll(Expression<Func<order_item, bool>> filter = null, params Expression<Func<order_item, object>>[] includes);
        public Task<order_item> GetAsync(Expression<Func<order_item, bool>> filter, params Expression<Func<order_item, object>>[] includes);
        public Task<order_item> GetByIdAsync(string id);
        public Task<int> GetCountAsync(Expression<Func<order_item, bool>> filter = null);
        public Task AddAsync(order_item entity);
        public void Delete(order_item entity) ;


    }
} 