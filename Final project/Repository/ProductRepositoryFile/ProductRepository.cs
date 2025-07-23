using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository.ProductRepositoryFile
{
    public class ProductRepository : IProductRepository
    {
        private readonly AmazonDBContext _context;
        public ProductRepository(AmazonDBContext context) { _context = context; }
        public IQueryable<product> GetAll(Expression<Func<product, bool>> filter = null, params Expression<Func<product, object>>[] includes)
        {
            IQueryable<product> query = _context.products;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }
        public async Task<product> GetAsync(Expression<Func<product, bool>> filter, params Expression<Func<product, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }
        public async Task<product> GetByIdAsync(string id) => await _context.products.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<product, bool>> filter = null)
        {
            if (filter != null)
                return await _context.products.CountAsync(filter);
            return await _context.products.CountAsync();
        }
        public async Task AddAsync(product entity) => await _context.products.AddAsync(entity);
        public void Add(product entity) => _context.products.Add(entity);
        public void Update(product entity) => _context.products.Update(entity);
        public void Delete(product entity) => _context.products.Remove(entity);
    }
}
