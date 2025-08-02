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

        // Updated Index action to handle search parameter and category name
        public IActionResult Index(CategoryFilter filter, string search = null, string categoryName = null)
        {
            // Pass search term to the view through the filter model
            if (!string.IsNullOrEmpty(search))
            {
                filter.SearchTerm = search;
            }

            // Handle category name parameter
            if (!string.IsNullOrEmpty(categoryName))
            {
                filter.categoryName = categoryName;

                // If categoryId is not provided but categoryName is, try to find the category ID
                if (string.IsNullOrEmpty(filter.categoryId))
                {
                    var categories = unitOfWork.CategoryRepository.GetCategoryWithItsChildern();
                    var matchingCategory = categories.FirstOrDefault(c =>
                        c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                    if (matchingCategory != null)
                    {
                        filter.categoryId = matchingCategory.Id.ToString();
                    }
                }
            }

            return View(filter);
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
            string sortBy = "relevance",
            string filter = null,
            string search = null,
            string categoryName = null)
        {
            try
            {
                int skip = (page - 1) * pageSize;

                // If categoryName is provided but categoryId is not, try to find the category ID
                if (!string.IsNullOrEmpty(categoryName) && string.IsNullOrEmpty(categoryId))
                {
                    var categories = unitOfWork.CategoryRepository.GetCategoryWithItsChildern();
                    var matchingCategory = categories.FirstOrDefault(c =>
                        c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                    if (matchingCategory != null)
                    {
                        categoryId = matchingCategory.Id.ToString();
                    }
                }

                // Handle special filters from landing page
                if (!string.IsNullOrEmpty(filter))
                {
                    switch (filter.ToLower())
                    {
                        case "discounts":
                            sortBy = "discount";
                            break;
                        case "bestsellers":
                            sortBy = "bestseller";
                            break;
                        case "newarrivals":
                            sortBy = "newest";
                            break;
                        case "todaysdeals":
                            sortBy = "todaysdeals";
                            break;
                    }
                }

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
                    Skip = skip,
                    Filter = filter,
                    SearchTerm = search // Search term is now handled in repository
                };

                // Get filtered products (now includes search filtering in repository)
                var products = unitOfWork.LandingPageReposotory.GetFilteredProducts(filterParams);

                // Apply additional filtering based on filter parameter if not handled in repository
                if (!string.IsNullOrEmpty(filter))
                {
                    switch (filter.ToLower())
                    {
                        case "discounts":
                            products = products.Where(p => p.DiscountPrice.HasValue && p.DiscountPrice < p.Price).ToList();
                            break;
                        case "bestsellers":
                            products = products.OrderByDescending(p => p.TotalSold).ToList();
                            break;
                        case "newarrivals":
                            products = products.OrderByDescending(p => p.delaviryTiming).ToList();
                            break;
                        case "todaysdeals":
                            products = products.Where(p => p.DiscountPrice.HasValue && p.DiscountPrice < p.Price)
                                             .OrderByDescending(p => p.DiscountPercentage ?? 0)
                                             .ToList();
                            break;
                    }
                }

                // Get total count with same filters (now includes search in repository)
                var totalProducts = unitOfWork.LandingPageReposotory.GetFilteredProductsCount(filterParams);

                // Apply same filtering for count if needed
                if (!string.IsNullOrEmpty(filter))
                {
                    switch (filter.ToLower())
                    {
                        case "discounts":
                        case "todaysdeals":
                            totalProducts = products.Count();
                            break;
                    }
                }

                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                // Calculate discount statistics for the current page
                var productsWithDiscounts = products.Where(p => p.DiscountPrice.HasValue).Count();
                var averageDiscountPercentage = products
                    .Where(p => p.DiscountPercentage.HasValue)
                    .Select(p => p.DiscountPercentage.Value)
                    .DefaultIfEmpty(0)
                    .Average();

                var response = new
                {
                    products = products.Select(p => new
                    {
                        productId = p.ProductId,
                        productName = p.ProductName,
                        imageUrl = p.ImageUrl,
                        originalPrice = Math.Round((decimal)p.Price,2),
                        discountPrice = Math.Round((decimal)p.DiscountPrice,2),
                        discountPercentage = p.DiscountPercentage,
                        hasDiscount = p.DiscountPrice.HasValue,
                        finalPrice = p.DiscountPrice ?? p.Price,
                        totalSold = p.TotalSold,
                        totalRevenue = p.TotalRevenue,
                        rating = p.ratting,
                        ratingStarMinus = p.rattingStarMinuse,
                        ratingCount = p.ratingCount,
                        deliveryTiming = p.delaviryTiming,
                        prime = p.prime,
                        description = p.Description
                    }),
                    pagination = new
                    {
                        currentPage = page,
                        totalPages = totalPages,
                        pageSize = pageSize,
                        totalProducts = totalProducts,
                        hasNextPage = page < totalPages,
                        hasPreviousPage = page > 1
                    },
                    statistics = new
                    {
                        productsWithDiscounts = productsWithDiscounts,
                        discountedProductsPercentage = totalProducts > 0 ? Math.Round((double)productsWithDiscounts / products.Count * 100, 1) : 0,
                        averageDiscountPercentage = Math.Round(averageDiscountPercentage, 1)
                    },
                    appliedFilters = new
                    {
                        categoryId = categoryId,
                        subcategoryId = subcategoryId,
                        minPrice = minPrice,
                        maxPrice = maxPrice,
                        minRating = minRating,
                        sortBy = sortBy,
                        filter = filter,
                        search = search,
                        categoryName = categoryName
                    }
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    message = "Failed to load products: " + ex.Message,
                    products = new List<object>(),
                    pagination = new
                    {
                        currentPage = page,
                        totalPages = 0,
                        pageSize = pageSize,
                        totalProducts = 0,
                        hasNextPage = false,
                        hasPreviousPage = false
                    }
                });
            }
        }
    }
}