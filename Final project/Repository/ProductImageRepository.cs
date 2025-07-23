using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AmazonDBContext _context;
        public ProductImageRepository(AmazonDBContext context) { _context = context; }
        public IQueryable<product_image> GetAll(Expression<Func<product_image, bool>> filter = null, params Expression<Func<product_image, object>>[] includes)
        {
            IQueryable<product_image> query = _context.product_images;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }
        public async Task<product_image> GetAsync(Expression<Func<product_image, bool>> filter, params Expression<Func<product_image, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }
        public async Task<product_image> GetByIdAsync(string id) => await _context.product_images.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<product_image, bool>> filter = null)
        {
            if (filter != null)
                return await _context.product_images.CountAsync(filter);
            return await _context.product_images.CountAsync();
        }
        public async Task AddAsync(product_image entity) => await _context.product_images.AddAsync(entity);
        public void Add(product_image entity) => _context.product_images.Add(entity);
        public void Update(product_image entity) => _context.product_images.Update(entity);
        public void Delete(product_image entity) => _context.product_images.Remove(entity);
        public void Remove(product_image entity) => Delete(entity);
    }
} 