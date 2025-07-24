using Final_project.Models;
using Final_project.Repository.Product;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.ProductRepositoryFile
{
    public class ProductRepository : IProductRepository
    {
        private readonly AmazonDBContext db;

        public ProductRepository(AmazonDBContext db)
        {
            this.db = db;
        }

        public List<product> getAll()
        {
            return db.products.ToList();
        }

        public product getById(string id)
        {
            return db.products.FirstOrDefault(p => p.id == id);
        }

        public void Update(product entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }

        public void add(product entity)
        {
            db.products.Add(entity);
        }

        public async Task<product> GetAsync(System.Linq.Expressions.Expression<System.Func<product, bool>> filter, params System.Linq.Expressions.Expression<System.Func<product, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }

        public async Task<product> GetByIdAsync(string id)
        {
            return await db.products.FirstOrDefaultAsync(p => p.id == id);
        }

        public async Task<int> GetCountAsync(System.Linq.Expressions.Expression<System.Func<product, bool>> filter = null)
        {
            if (filter != null)
                return await db.products.CountAsync(filter);
            return await db.products.CountAsync();
        }

        public async Task AddAsync(product entity)
        {
            await db.products.AddAsync(entity);
        }

        public void Delete(product entity)
        {
            db.products.Remove(entity);
        }

        public IQueryable<product> GetAll(System.Linq.Expressions.Expression<System.Func<product, bool>> filter = null, params System.Linq.Expressions.Expression<System.Func<product, object>>[] includes)
        {
            IQueryable<product> query = db.products;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }
    }
}
