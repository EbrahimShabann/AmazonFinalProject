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

        // Updated Index action to accept categoryId parameter and filter
        public IActionResult Index(CategoryFilter filter)
        {
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
    string filter = null) // New filter parameter
        {
            try
            {
                int skip = (page - 1) * pageSize;

                // Handle special filters from landing page
                if (!string.IsNullOrEmpty(filter))
                {
                    switch (filter.ToLower())
                    {
                        case "discounts":
                            // Filter for products with discounts
                            sortBy = "discount"; // You might need to implement this sort option
                            break;
                        case "bestsellers":
                            sortBy = "bestseller"; // Sort by best selling products
                            break;
                        case "newarrivals":
                            sortBy = "newest"; // Sort by newest arrivals
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
                    Filter = filter // Add filter to parameters if your repository supports it
                };

                // Get filtered products (now includes discount properties)
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
                            // Assuming you have a way to identify best sellers
                            products = products.OrderByDescending(p => p.TotalSold).ToList();
                            break;
                        case "newarrivals":
                            // Assuming you have a creation date or similar field
                            products = products.OrderByDescending(p => p.delaviryTiming).ToList();
                            break;
                    }
                }

                // Get total count for pagination info with same filters
                var totalProducts = unitOfWork.LandingPageReposotory.GetFilteredProductsCount(filterParams);

                // Apply same filtering for count if needed
                if (!string.IsNullOrEmpty(filter))
                {
                    switch (filter.ToLower())
                    {
                        case "discounts":
                            // You might need to implement a separate count method that handles discounts
                            totalProducts = products.Count(); // Temporary solution
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
                        originalPrice = p.Price,
                        discountPrice = p.DiscountPrice,
                        discountPercentage = p.DiscountPercentage,
                        hasDiscount = p.DiscountPrice.HasValue,
                        finalPrice = p.DiscountPrice ?? p.Price, // The actual price to display
                        totalSold = p.TotalSold,
                        totalRevenue = p.TotalRevenue,
                        rating = p.ratting,
                        ratingStarMinus = p.rattingStarMinuse,
                        ratingCount = p.ratingCount,
                        deliveryTiming = p.delaviryTiming,
                        prime = p.prime
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
                        filter = filter // Include the filter in response
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
