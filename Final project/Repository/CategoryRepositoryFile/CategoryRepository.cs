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
    }
}
