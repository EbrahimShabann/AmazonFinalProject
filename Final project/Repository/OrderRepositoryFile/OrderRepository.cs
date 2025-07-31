using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AmazonDBContext _context;
        public OrderRepository(AmazonDBContext context) { _context = context; }
        public IQueryable<order> GetAll(Expression<Func<order, bool>> filter = null, params Expression<Func<order, object>>[] includes)
        {
            IQueryable<order> query = _context.orders;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }
        public async Task<order> GetAsync(Expression<Func<order, bool>> filter, params Expression<Func<order, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }
        public async Task<order> GetByIdAsync(string id) => await _context.orders.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<order, bool>> filter = null)
        {
            if (filter != null)
                return await _context.orders.CountAsync(filter);
            return await _context.orders.CountAsync();
        }
        public async Task AddAsync(order entity) => await _context.orders.AddAsync(entity);
        public void add(order entity) => _context.orders.Add(entity);
        public void Update(order entity) => _context.orders.Update(entity);
        public void Delete(order entity) => _context.orders.Remove(entity);

        public List<order> getAll()
        {
            throw new NotImplementedException();
        }

        public order getById(string id)
        {
            throw new NotImplementedException();
        }

    }
} 