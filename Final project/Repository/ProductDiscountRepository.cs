using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class ProductDiscountRepository : IProductDiscountRepository
    {
        private readonly AmazonDBContext _context;
        public ProductDiscountRepository(AmazonDBContext context) { _context = context; }
        public IQueryable<product_discount> GetAll(Expression<Func<product_discount, bool>> filter = null, params Expression<Func<product_discount, object>>[] includes)
        {
            IQueryable<product_discount> query = _context.product_discounts;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }
        public async Task<product_discount> GetAsync(Expression<Func<product_discount, bool>> filter, params Expression<Func<product_discount, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }
        public async Task<product_discount> GetByIdAsync(string id) => await _context.product_discounts.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<product_discount, bool>> filter = null)
        {
            if (filter != null)
                return await _context.product_discounts.CountAsync(filter);
            return await _context.product_discounts.CountAsync();
        }
        public async Task AddAsync(product_discount entity) => await _context.product_discounts.AddAsync(entity);
        public void Add(product_discount entity) => _context.product_discounts.Add(entity);
        public void Update(product_discount entity) => _context.product_discounts.Update(entity);
        public void Delete(product_discount entity) => _context.product_discounts.Remove(entity);
    }
} 