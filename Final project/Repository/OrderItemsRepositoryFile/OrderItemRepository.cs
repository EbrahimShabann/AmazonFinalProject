using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AmazonDBContext _context;
        public OrderItemRepository(AmazonDBContext context) { _context = context; }
        public IQueryable<order_item> GetAll(Expression<Func<order_item, bool>> filter = null, params Expression<Func<order_item, object>>[] includes)
        {
            IQueryable<order_item> query = _context.order_items;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }
        public async Task<order_item> GetAsync(Expression<Func<order_item, bool>> filter, params Expression<Func<order_item, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }
        public async Task<order_item> GetByIdAsync(string id) => await _context.order_items.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<order_item, bool>> filter = null)
        {
            if (filter != null)
                return await _context.order_items.CountAsync(filter);
            return await _context.order_items.CountAsync();
        }
        public async Task AddAsync(order_item entity) => await _context.order_items.AddAsync(entity);
        public void add(order_item entity) => _context.order_items.Add(entity);
        public void Update(order_item entity) => _context.order_items.Update(entity);
        public void Delete(order_item entity) => _context.order_items.Remove(entity);

        public List<order_item> getAll()
        {
            throw new NotImplementedException();
        }

        public order_item getById(string id)
        {
            throw new NotImplementedException();
        }


    }
} 