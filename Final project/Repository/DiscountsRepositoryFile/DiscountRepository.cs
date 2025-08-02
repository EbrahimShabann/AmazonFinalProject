using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AmazonDBContext _context;
        public DiscountRepository(AmazonDBContext context) { _context = context; }

        public IQueryable<discount> GetAll(Expression<Func<discount, bool>> filter = null, params Expression<Func<discount, object>>[] includes)
        {
            IQueryable<discount> query = _context.discounts;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }

        public async Task<discount> GetAsync(Expression<Func<discount, bool>> filter, params Expression<Func<discount, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }

        public async Task<discount> GetByIdAsync(string id) => await _context.discounts.FindAsync(id);

        public async Task<int> GetCountAsync(Expression<Func<discount, bool>> filter = null)
        {
            if (filter != null)
                return await _context.discounts.CountAsync(filter);
            return await _context.discounts.CountAsync();
        }

        public async Task AddAsync(discount entity) => await _context.discounts.AddAsync(entity);
        public void add(discount entity) => _context.discounts.Add(entity);
        public void Update(discount entity) => _context.discounts.Update(entity);
        public void Delete(discount entity) => _context.discounts.Remove(entity);

        public List<discount> getAll()
        {
            throw new NotImplementedException();
        }

        public discount getById(string id)
        {
            throw new NotImplementedException();
        }


    }
} 