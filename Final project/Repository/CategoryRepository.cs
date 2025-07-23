using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AmazonDBContext _context;
        public CategoryRepository(AmazonDBContext context) { _context = context; }
        public IQueryable<category> GetAll(Expression<Func<category, bool>> filter = null, params Expression<Func<category, object>>[] includes)
        {
            IQueryable<category> query = _context.categories;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }
        public async Task<category> GetAsync(Expression<Func<category, bool>> filter, params Expression<Func<category, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }
        public async Task<category> GetByIdAsync(string id) => await _context.categories.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<category, bool>> filter = null)
        {
            if (filter != null)
                return await _context.categories.CountAsync(filter);
            return await _context.categories.CountAsync();
        }
        public async Task AddAsync(category entity) => await _context.categories.AddAsync(entity);
        public void Add(category entity) => _context.categories.Add(entity);
        public void Update(category entity) => _context.categories.Update(entity);
        public void Delete(category entity) => _context.categories.Remove(entity);
    }
} 