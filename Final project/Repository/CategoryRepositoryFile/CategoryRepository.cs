using System.Linq.Expressions;
using Final_project.Models;
using Final_project.ViewModel.LandingPageViewModels;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CategoryFile
{
    public class CategoryRepository :ICategoryRepository
    {
        private readonly AmazonDBContext db;

        public CategoryRepository(AmazonDBContext db)
        {
            this.db = db;
        }

        public List<CategoryViewModel> GetCategoryWithItsChildern()
        {
            var parentCategories = db.categories
                           .Where(c => c.parent_category_id == null)
                           .OrderBy(c => c.name)
                           .ToList();

            var result = new List<CategoryViewModel>();

            foreach (var parentCategory in parentCategories)
            {
                var parentViewModel = new CategoryViewModel
                {
                    Id = parentCategory.id,
                    Name = parentCategory.name,
                    Description = parentCategory.description,
                    ImageUrl = parentCategory.image_url,
                    ParentCategoryName = null 
                };

                var childCategories = db.categories
                    .Where(c => c.parent_category_id == parentCategory.id)
                    .OrderBy(c => c.name)
                    .ToList();

                foreach (var childCategory in childCategories)
                {
                    parentViewModel.ChildCategories.Add(new CategoryViewModel
                    {
                        Id = childCategory.id,
                        Name = childCategory.name,
                        Description = childCategory.description,
                        ImageUrl = childCategory.image_url,
                        ParentCategoryName = parentCategory.name
                    });
                }

                result.Add(parentViewModel);
            }

            return result;
        }

        public int totalProduct()
        {
            return db.order_items
                    .Include(oi => oi.product)
                    .Where(oi => oi.quantity.HasValue && oi.product != null)
                    .GroupBy(oi => oi.product_id)
                    .Count();
        }


        public IQueryable<category> GetAll(Expression<Func<category, bool>> filter = null, params Expression<Func<category, object>>[] includes)
        {
            IQueryable<category> query = db.categories;
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
        public async Task<category> GetByIdAsync(string id) => await db.categories.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<category, bool>> filter = null)
        {
            if (filter != null)
                return await db.categories.CountAsync(filter);
            return await db.categories.CountAsync();
        }
        public async Task AddAsync(category entity) => await db.categories.AddAsync(entity);
        public void add(category entity) => db.categories.Add(entity);
        public void Update(category entity) => db.categories.Update(entity);
        public void Delete(category entity) => db.categories.Remove(entity);

        public List<category> getAll()
        {
            throw new NotImplementedException();
        }

        public category getById(string id)
        {
            throw new NotImplementedException();
        }


    }
}
