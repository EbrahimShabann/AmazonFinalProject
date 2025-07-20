using Final_project.Repository;
using Final_project.ViewModel.LandingPageViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class CategoryController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public CategoryController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // Updated Index action to accept categoryId parameter
        public IActionResult Index(string categoryId = null, string categoryName = null)
        {
            // Pass categoryId and categoryName to the view if provided
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = categoryName;

            return View();
        }

        [HttpGet]
        public IActionResult GetCategorys()
        {
            return Json(unitOfWork.CategoryRepository.GetCategoryWithItsChildern());
        }

        [HttpGet]
        public IActionResult GetPaginatedProducts(
            int page = 1,
            int pageSize = 20,
            string categoryId = null,
            string subcategoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minRating = null,
            string sortBy = "relevance")
        {
            try
            {
                int skip = (page - 1) * pageSize;

                // Create filter parameters object
                var filterParams = new ProductFilterParameters
                {
                    CategoryId = categoryId,
                    SubcategoryId = subcategoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MinRating = minRating,
                    SortBy = sortBy,
                    PageSize = pageSize,
                    Skip = skip
                };

                // Get filtered products
                var products = unitOfWork.LandingPageReposotory.GetFilteredProducts(filterParams);

                // Get total count for pagination info with same filters
                var totalProducts = unitOfWork.LandingPageReposotory.GetFilteredProductsCount(filterParams);
                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                var response = new
                {
                    products = products,
                    currentPage = page,
                    totalPages = totalPages,
                    pageSize = pageSize,
                    totalProducts = totalProducts,
                    hasNextPage = page < totalPages,
                    hasPreviousPage = page > 1,
                    appliedFilters = new
                    {
                        categoryId = categoryId,
                        subcategoryId = subcategoryId,
                        minPrice = minPrice,
                        maxPrice = maxPrice,
                        minRating = minRating,
                        sortBy = sortBy
                    }
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    message = "Failed to load products: " + ex.Message
                });
            }
        }


    }
}
