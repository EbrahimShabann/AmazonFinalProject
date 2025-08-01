using Final_project.Models;
using Final_project.Repository.NewFolder;
using Final_project.ViewModel.LandingPageViewModels;
using Final_project.ViewModel.NewFolder;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.LandingPageFile
{
    public class LandingPageRepository:ILandingPageRepository
    {
        private readonly AmazonDBContext db;

        public LandingPageRepository(AmazonDBContext db)
        {
            this.db = db;
        }
        private static readonly Random _random = new Random();

        public List<LandingPageProducts> GetBestSellers(int take = 10, int skip = 0)
        {
            // Get top products from database with pagination
            var topProducts = db.order_items
                .Include(oi => oi.product)
                .Where(oi => oi.quantity.HasValue && oi.product != null)
                .GroupBy(oi => oi.product_id)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(oi => oi.quantity.Value),
                    TotalRevenue = g.Sum(oi => (oi.quantity.Value * oi.unit_price.Value) -
                                             (oi.discount_applied.HasValue ? oi.discount_applied.Value : 0)),
                    Product = g.First().product
                })
                .OrderByDescending(x => x.TotalSold)
                .Skip(skip)
                .Take(take)
                .ToList();

            // Process each product synchronously
            var landingPageProducts = new List<LandingPageProducts>();

            foreach (var product in topProducts)
            {
                var data = new LandingPageProducts
                {
                    ProductId = product.ProductId,
                    ProductName = product.Product.name,
                    Price = product.Product.price,
                    TotalSold = product.TotalSold,
                    TotalRevenue = product.TotalRevenue,
                    ImageUrl = GetProductImageUrl(product.ProductId),
                    ratting = GetProductRating(product.ProductId),
                    ratingCount = GetProductRatingCount(product.ProductId),
                    delaviryTiming = DateTime.Now.AddDays(_random.Next(1, 4)), // Random 1-3 days
                    prime = _random.Next(2) == 1 // Random true/false
                };
                data.rattingStarMinuse = 5 - data.ratting;
                landingPageProducts.Add(data);
            }

            return landingPageProducts;
        }

        public List<LandingPageProducts> GetNewArrivals(int take = 10, int skip = 0)
        {
            // Get newest products from database with pagination
            var newProducts = db.products
                .Where(p => p.is_active == true &&
                           p.is_approved == true &&
                           p.is_deleted == false)
                .OrderByDescending(p => p.created_at)
                .Skip(skip)
                .Take(take)
                .ToList();

            // Process each product
            var landingPageProducts = new List<LandingPageProducts>();

            foreach (var product in newProducts)
            {
                var data = new LandingPageProducts
                {
                    ProductId = product.id,
                    ProductName = product.name,
                    Price = product.price,
                    ImageUrl = GetProductImageUrl(product.id),
                    ratting = GetProductRating(product.id),
                    ratingCount = GetProductRatingCount(product.id),
                    delaviryTiming = DateTime.Now.AddDays(_random.Next(1, 4)), // Random 1-3 days
                    prime = _random.Next(2) == 1 // Random true/false
                };
                data.rattingStarMinuse = 5 - data.ratting;
                landingPageProducts.Add(data);
            }

            return landingPageProducts;
        }

        public List<LandingPageProductDiscount> GetNewDiscounts(int take = 10, int skip = 0)
        {
            var currentDate = DateTime.UtcNow;

            // Get newest discounted products with their discount information with pagination
            var discountedProducts = db.product_discounts
                .Include(pd => pd.product)
                .Include(pd => pd.Discount)
                .Where(pd => pd.product.is_active == true &&
                            pd.product.is_approved == true &&
                            pd.product.is_deleted == false &&
                            pd.Discount.is_active == true &&
                            pd.Discount.start_date <= currentDate &&
                            (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate))
                .OrderByDescending(pd => pd.Discount.created_at) // Newest discounts first
                .Skip(skip)
                .Take(take)
                .Select(pd => new
                {
                    Product = pd.product,
                    Discount = pd.Discount,
                    TotalSold = db.order_items
                                 .Where(oi => oi.product_id == pd.product_id)
                                 .Sum(oi => oi.quantity) ?? 0,
                    TotalRevenue = db.order_items
                                   .Where(oi => oi.product_id == pd.product_id)
                                   .Sum(oi => oi.quantity * oi.unit_price) ?? 0
                })
                .ToList();

            var result = new List<LandingPageProductDiscount>();

            foreach (var item in discountedProducts)
            {
                // Calculate the discounted price based on discount type
                decimal? discountedPrice = item.Product.price;
                if (item.Discount.discount_type == "Percentage" && item.Discount.value.HasValue)
                {
                    discountedPrice = item.Product.price * (1 - item.Discount.value.Value / 100);
                }
                else if (item.Discount.discount_type == "Fixed" && item.Discount.value.HasValue)
                {
                    discountedPrice = item.Product.price - item.Discount.value.Value;
                }

                var data = new LandingPageProductDiscount
                {
                    ProductId = item.Product.id,
                    ProductName = item.Product.name,
                    ImageUrl = GetProductImageUrl(item.Product.id),
                    Price = Math.Round((decimal)item.Product.price, 2),
                    DiscountPrice = Math.Round((decimal)discountedPrice,2),
                    TotalSold = (int)item.TotalSold,
                    ratting = GetProductRating(item.Product.id),
                    ratingCount = GetProductRatingCount(item.Product.id),
                    delaviryTiming = DateTime.Now.AddDays(_random.Next(1, 4)), // Random 1-3 days
                    prime = _random.Next(2) == 1 // Random true/false
                };
                data.rattingStarMinuse = 5 - data.ratting;
                result.Add(data);
            }

            return result;
        }
        public int GetBestSellersCount()
        {
            return db.order_items
                .Include(oi => oi.product)
                .Where(oi => oi.quantity.HasValue && oi.product != null)
                .GroupBy(oi => oi.product_id)
                .Count();
        }

        public int GetNewArrivalsCount()
        {
            return db.products
                .Where(p => p.is_active == true &&
                           p.is_approved == true &&
                           p.is_deleted == false)
                .Count();
        }

        public int GetNewDiscountsCount()
        {
            var currentDate = DateTime.UtcNow;

            return db.product_discounts
                .Include(pd => pd.product)
                .Include(pd => pd.Discount)
                .Where(pd => pd.product.is_active == true &&
                            pd.product.is_approved == true &&
                            pd.product.is_deleted == false &&
                            pd.Discount.is_active == true &&
                            pd.Discount.start_date <= currentDate &&
                            (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate))
                .Count();
        }
        public string GetProductImageUrl(string productId)
        {
            try
            {
                return db.product_images
                    .Where(pi => pi.product_id == productId && pi.is_primary == true)
                    .Select(pi => pi.image_url)
                    .FirstOrDefault() ?? "https://imageplaceholder.net/600x400/eeeeee/131313?text=Not+Found";
            }
            catch
            {
                return "https://imageplaceholder.net/600x400/eeeeee/131313?text=Not+Found";
            }
        }

        public int GetProductRating(string productId)
        {
            try
            {
                var averageRating = db.product_reviews
                    .Where(pr => pr.product_id == productId)
                    .Average(r => (double?)r.rating); // Note the (double?) cast

                return averageRating.HasValue ? (int)Math.Round(averageRating.Value) : 0;
            }
            catch
            {
                return 0;
            }
        }

        public int GetProductRatingCount(string productId)
        {
            try
            {
                return db.product_reviews.Count(pr => pr.product_id == productId);
            }
            catch
            {
                return 0;
            }
        }

        public List<ProductSearchViewModel> ProductSearch(string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Add null/empty checks for searchTerm
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return new List<ProductSearchViewModel>();
                }

                // Ensure db and db.products are not null
                if (db?.products == null)
                {
                    return new List<ProductSearchViewModel>();
                }

                var currentDate = DateTime.UtcNow;

                // Get products with complete information including price and discount data
                var products = db.products
                    .Where(p => p.is_active == true &&
                               p.is_approved == true &&
                               p.is_deleted == false &&
                               p.price.HasValue && // Ensure product has a price
                               (p.name.Contains(searchTerm) ||
                                (p.description != null && p.description.Contains(searchTerm)) ||
                                (p.Brand != null && p.Brand.Contains(searchTerm))))
                    .OrderByDescending(p => p.created_at) // Show newest first
                    .ThenBy(p => p.name) // Then alphabetically
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Check if products is null before processing
                if (products == null)
                {
                    return new List<ProductSearchViewModel>();
                }

                var result = new List<ProductSearchViewModel>();

                foreach (var product in products)
                {
                    // Check for active discounts
                    decimal? discountedPrice = null;

                    // First check if product has direct discount_price
                    if (product.discount_price.HasValue)
                    {
                        discountedPrice = product.discount_price.Value;
                    }
                    else
                    {
                        // Check the discount system for active discounts
                        var activeDiscount = db.product_discounts
                            .Include(pd => pd.Discount)
                            .Where(pd => pd.product_id == product.id &&
                                        pd.Discount.is_active == true &&
                                        pd.Discount.start_date <= currentDate &&
                                        (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate))
                            .FirstOrDefault();

                        if (activeDiscount != null && activeDiscount.Discount.value.HasValue)
                        {
                            if (activeDiscount.Discount.discount_type == "Percentage")
                            {
                                discountedPrice = product.price.Value * (1 - activeDiscount.Discount.value.Value / 100);
                            }
                            else if (activeDiscount.Discount.discount_type == "Fixed")
                            {
                                discountedPrice = product.price.Value - activeDiscount.Discount.value.Value;
                            }

                            // Ensure discount price is not negative
                            if (discountedPrice < 0)
                                discountedPrice = 0;
                        }
                    }

                    var viewModel = new ProductSearchViewModel
                    {
                        ProductId = product.id,
                        ProductName = product.name ?? "Product",
                        ImageUrl = GetProductImageUrl(product.id),
                        Price = Math.Round((double)(product.price ?? 0), 2),
                        DiscountPrice = discountedPrice.HasValue ? Math.Round((double)discountedPrice.Value, 2) : (double?)null,
                        Brand = product.Brand ?? "Amazon",
                        Rating = GetProductRating(product.id),
                        ReviewCount = GetProductRatingCount(product.id),
                        Description = product.description,
                        Prime = _random.Next(2) == 1 // Random for now, implement actual Prime logic as needed
                    };

                    result.Add(viewModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception (replace with your logging mechanism)
                Console.WriteLine($"Error in ProductSearch method: {ex.Message}");

                // Return empty list instead of throwing
                return new List<ProductSearchViewModel>();
            }
        }


        public List<LandingPageProducts> GetFilteredProducts(ProductFilterParameters filterParams)
        {
            try
            {
                // Handle special filter for Today's Deals
                if (!string.IsNullOrEmpty(filterParams.Filter) && filterParams.Filter.ToLower() == "todaysdeals")
                {
                    return GetTodaysDeals(filterParams.PageSize, filterParams.Skip);
                }

                // Start with base query
                var query = db.products
                    .Where(p => p.is_active == true &&
                               p.is_approved == true &&
                               p.is_deleted == false);

                // Apply search filter - ADD THIS SECTION
                if (!string.IsNullOrEmpty(filterParams.SearchTerm))
                {
                    var searchTerm = filterParams.SearchTerm.ToLower();
                    query = query.Where(p =>
                        p.name.ToLower().Contains(searchTerm) ||
                        (p.description != null && p.description.ToLower().Contains(searchTerm)));
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(filterParams.SubcategoryId))
                {
                    query = query.Where(p => p.category_id == filterParams.SubcategoryId);
                }
                else if (!string.IsNullOrEmpty(filterParams.CategoryId) && filterParams.CategoryId != "all")
                {
                    // Get all subcategory IDs for the parent category
                    var subcategoryIds = db.categories
                        .Where(c => c.parent_category_id == filterParams.CategoryId &&
                                   c.is_active == true &&
                                   c.is_deleted == false)
                        .Select(c => c.id)
                        .ToList();

                    // Include parent category and all its subcategories
                    subcategoryIds.Add(filterParams.CategoryId);
                    query = query.Where(p => subcategoryIds.Contains(p.category_id));
                }

                // Apply price filter
                if (filterParams.MinPrice.HasValue)
                {
                    query = query.Where(p => p.price >= filterParams.MinPrice.Value);
                }

                if (filterParams.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.price <= filterParams.MaxPrice.Value);
                }

                // Apply rating filter (requires joining with reviews)
                if (filterParams.MinRating.HasValue)
                {
                    var productsWithMinRating = db.product_reviews
                        .GroupBy(pr => pr.product_id)
                        .Where(g => g.Average(r => r.rating) >= filterParams.MinRating.Value)
                        .Select(g => g.Key)
                        .ToList();

                    query = query.Where(p => productsWithMinRating.Contains(p.id));
                }

                // Apply sorting
                query = ApplySorting(query, filterParams.SortBy);

                // Apply pagination
                var products = query
                    .Skip(filterParams.Skip)
                    .Take(filterParams.PageSize)
                    .ToList();

                // Get current date for discount validation
                var currentDate = DateTime.UtcNow;

                // Convert to LandingPageProducts
                var landingPageProducts = new List<LandingPageProducts>();

                foreach (var product in products)
                {
                    // Check if product has active discount
                    var activeDiscount = db.product_discounts
                        .Include(pd => pd.Discount)
                        .Where(pd => pd.product_id == product.id &&
                                    pd.Discount.is_active == true &&
                                    pd.Discount.start_date <= currentDate &&
                                    (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate))
                        .FirstOrDefault();

                    // Calculate discounted price if discount exists
                    decimal? discountedPrice = null;
                    decimal? discountPercentage = null;

                    if (activeDiscount != null && activeDiscount.Discount.value.HasValue)
                    {
                        if (activeDiscount.Discount.discount_type == "Percentage")
                        {
                            discountPercentage = activeDiscount.Discount.value.Value;
                            discountedPrice = product.price * (1 - activeDiscount.Discount.value.Value / 100);
                        }
                        else if (activeDiscount.Discount.discount_type == "Fixed")
                        {
                            discountedPrice = product.price - activeDiscount.Discount.value.Value;
                            discountPercentage = (decimal?)((activeDiscount.Discount.value.Value / product.price) * 100);
                        }

                        // Ensure discounted price is not negative
                        if (discountedPrice < 0)
                            discountedPrice = 0;
                    }

                    var data = new LandingPageProducts
                    {
                        ProductId = product.id,
                        ProductName = product.name,
                        Price = Math.Round((decimal)product.price,2),
                        DiscountPrice = Math.Round((decimal)discountedPrice,2),
                        ImageUrl = GetProductImageUrl(product.id),
                        ratting = GetProductRating(product.id),
                        ratingCount = GetProductRatingCount(product.id),
                        delaviryTiming = DateTime.Now.AddDays(_random.Next(1, 4)),
                        prime = _random.Next(2) == 1,
                        Description = product.description,
                        TotalSold = GetProductSalesCount(product.id)
                    };
                    data.rattingStarMinuse = 5 - data.ratting;
                    landingPageProducts.Add(data);
                }

                return landingPageProducts;
            }
            catch (Exception ex)
            {
                // Log exception
                return new List<LandingPageProducts>();
            }
        }

        public int GetFilteredProductsCount(ProductFilterParameters filterParams)
        {
            try
            {
                // Start with base query
                var query = db.products
                    .Where(p => p.is_active == true &&
                               p.is_approved == true &&
                               p.is_deleted == false);

                // Apply search filter - ADD THIS SECTION
                if (!string.IsNullOrEmpty(filterParams.SearchTerm))
                {
                    var searchTerm = filterParams.SearchTerm.ToLower();
                    query = query.Where(p =>
                        p.name.ToLower().Contains(searchTerm) ||
                        (p.description != null && p.description.ToLower().Contains(searchTerm)));
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(filterParams.SubcategoryId))
                {
                    query = query.Where(p => p.category_id == filterParams.SubcategoryId);
                }
                else if (!string.IsNullOrEmpty(filterParams.CategoryId) && filterParams.CategoryId != "all")
                {
                    var subcategoryIds = db.categories
                        .Where(c => c.parent_category_id == filterParams.CategoryId &&
                                   c.is_active == true &&
                                   c.is_deleted == false)
                        .Select(c => c.id)
                        .ToList();

                    subcategoryIds.Add(filterParams.CategoryId);
                    query = query.Where(p => subcategoryIds.Contains(p.category_id));
                }

                // Apply price filter
                if (filterParams.MinPrice.HasValue)
                {
                    query = query.Where(p => p.price >= filterParams.MinPrice.Value);
                }

                if (filterParams.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.price <= filterParams.MaxPrice.Value);
                }

                // Apply rating filter
                if (filterParams.MinRating.HasValue)
                {
                    var productsWithMinRating = db.product_reviews
                        .GroupBy(pr => pr.product_id)
                        .Where(g => g.Average(r => r.rating) >= filterParams.MinRating.Value)
                        .Select(g => g.Key)
                        .ToList();

                    query = query.Where(p => productsWithMinRating.Contains(p.id));
                }

                return query.Count();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private IQueryable<product> ApplySorting(IQueryable<product> query, string sortBy)
        {
            switch (sortBy?.ToLower())
            {
                case "price-low":
                case "pricelowtohigh":
                    return query.OrderBy(p => p.price);

                case "price-high":
                case "pricehightolow":
                    return query.OrderByDescending(p => p.price);

                case "newest":
                case "newestarrivals":
                    return query.OrderByDescending(p => p.created_at);

                case "todaysdeals":
                    // For today's deals, we want to prioritize products with the highest discounts
                    var currentDate = DateTime.UtcNow;
                    var productsWithHighDiscounts = db.product_discounts
                        .Include(pd => pd.Discount)
                        .Where(pd => pd.Discount.is_active == true &&
                                    pd.Discount.start_date <= currentDate &&
                                    (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate))
                        .OrderByDescending(pd => pd.Discount.value)
                        .Select(pd => pd.product_id)
                        .ToList();

                    return query.OrderBy(p => productsWithHighDiscounts.IndexOf(p.id) == -1 ? int.MaxValue : productsWithHighDiscounts.IndexOf(p.id));

                case "bestselling":
                    // Join with order_items to sort by sales
                    var bestSellingProductIds = db.order_items
                        .Where(oi => oi.quantity.HasValue)
                        .GroupBy(oi => oi.product_id)
                        .OrderByDescending(g => g.Sum(oi => oi.quantity.Value))
                        .Select(g => g.Key)
                        .ToList();

                    return query.OrderBy(p => bestSellingProductIds.IndexOf(p.id));

                case "highestrated":
                    // Join with reviews to sort by rating
                    var ratedProductIds = db.product_reviews
                        .GroupBy(pr => pr.product_id)
                        .OrderByDescending(g => g.Average(r => r.rating))
                        .Select(g => g.Key)
                        .ToList();

                    return query.OrderBy(p => ratedProductIds.IndexOf(p.id));

                case "relevance":
                case "featured":
                default:
                    // Default sorting - could be by a combination of factors
                    return query.OrderByDescending(p => p.created_at)
                               .ThenByDescending(p => p.price);
            }
        }

        private int GetProductSalesCount(string productId)
        {
            try
            {
                return db.order_items
                    .Where(oi => oi.product_id == productId && oi.quantity.HasValue)
                    .Sum(oi => oi.quantity.Value);
            }
            catch
            {
                return 0;
            }
        }



        public List<LandingPageProducts> GetTodaysDeals(int take = 10, int skip = 0)
        {
            var currentDate = DateTime.UtcNow;
            var todayStart = currentDate.Date;
            var todayEnd = todayStart.AddDays(1).AddTicks(-1);

            // Get products with discounts that were created today or are specially marked as today's deals
            var todaysDealsProducts = db.product_discounts
                .Include(pd => pd.product)
                .Include(pd => pd.Discount)
                .Where(pd => pd.product.is_active == true &&
                            pd.product.is_approved == true &&
                            pd.product.is_deleted == false &&
                            pd.Discount.is_active == true &&
                            pd.Discount.start_date <= currentDate &&
                            (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate) &&
                            // Today's deals can be either created today or have high discount percentage
                            (pd.Discount.created_at >= todayStart || pd.Discount.value >= 20)) // 20% or more discount
                .OrderByDescending(pd => pd.Discount.value) // Order by highest discount first
                .ThenByDescending(pd => pd.Discount.created_at) // Then by newest
                .Skip(skip)
                .Take(take)
                .Select(pd => new
                {
                    Product = pd.product,
                    Discount = pd.Discount,
                    TotalSold = db.order_items
                                 .Where(oi => oi.product_id == pd.product_id)
                                 .Sum(oi => oi.quantity) ?? 0,
                    TotalRevenue = db.order_items
                                   .Where(oi => oi.product_id == pd.product_id)
                                   .Sum(oi => oi.quantity * oi.unit_price) ?? 0
                })
                .ToList();

            var result = new List<LandingPageProducts>();

            foreach (var item in todaysDealsProducts)
            {
                // Calculate the discounted price based on discount type
                decimal? discountedPrice = item.Product.price;
                decimal? discountPercentage = null;

                if (item.Discount.discount_type == "Percentage" && item.Discount.value.HasValue)
                {
                    discountPercentage = item.Discount.value.Value;
                    discountedPrice = item.Product.price * (1 - item.Discount.value.Value / 100);
                }
                else if (item.Discount.discount_type == "Fixed" && item.Discount.value.HasValue)
                {
                    discountedPrice = item.Product.price - item.Discount.value.Value;
                    // Calculate percentage for fixed discount - FIXED: Cast to decimal? instead of using Math.Round
                    discountPercentage = (decimal?)((item.Discount.value.Value / item.Product.price) * 100);
                }

                // Ensure discounted price is not negative
                if (discountedPrice < 0)
                    discountedPrice = 0;

                var data = new LandingPageProducts
                {
                    ProductId = item.Product.id,
                    ProductName = item.Product.name,
                    ImageUrl = GetProductImageUrl(item.Product.id),
                    Price = Math.Round((decimal)item.Product.price, 2),
                    DiscountPrice = discountedPrice,
                    // REMOVED: DiscountPercentage assignment - property appears to be read-only
                    TotalSold = (int)item.TotalSold,
                    TotalRevenue = (decimal)item.TotalRevenue,
                    ratting = GetProductRating(item.Product.id),
                    ratingCount = GetProductRatingCount(item.Product.id),
                    delaviryTiming = DateTime.Now.AddDays(_random.Next(1, 4)), // Random 1-3 days
                    prime = _random.Next(2) == 1 // Random true/false
                };
                data.rattingStarMinuse = 5 - data.ratting;
                result.Add(data);
            }

            return result;
        }

        public int GetTodaysDealsCount()
        {
            var currentDate = DateTime.UtcNow;
            var todayStart = currentDate.Date;

            return db.product_discounts
                .Include(pd => pd.product)
                .Include(pd => pd.Discount)
                .Where(pd => pd.product.is_active == true &&
                            pd.product.is_approved == true &&
                            pd.product.is_deleted == false &&
                            pd.Discount.is_active == true &&
                            pd.Discount.start_date <= currentDate &&
                            (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate) &&
                            (pd.Discount.created_at >= todayStart || pd.Discount.value >= 20))
                .Count();
        }

        public int GetCartCount(string UserName)
        {
            var userId = db.Users.FirstOrDefault(u => u.UserName == UserName).Id;
            return db.cart_items.Include(c => c.Cart)
                         .Include(c => c.Product)
                         .Where(c => c.Cart.user_id == userId && c.Product.is_active == true &&
                                   c.Product.is_approved == true && c.Product.is_deleted == false)
                         .Select(ci => ci.quantity)
                         .Sum() ?? 0;
        }


        public List<ChatbotProductViewModel> GetProductsForChatbot(string searchTerm, int pageNumber = 1, int pageSize = 8)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return new List<ChatbotProductViewModel>();
                }

                if (db?.products == null)
                {
                    return new List<ChatbotProductViewModel>();
                }

                var currentDate = DateTime.UtcNow;

                // Get products with complete information, leveraging the existing model structure
                var products = db.products
                    .Where(p => p.is_active == true &&
                               p.is_approved == true &&
                               p.is_deleted == false &&
                               p.price.HasValue && // Ensure product has a price
                               (p.name.Contains(searchTerm) ||
                                (p.description != null && p.description.Contains(searchTerm)) ||
                                (p.Brand != null && p.Brand.Contains(searchTerm))))
                    .OrderByDescending(p => p.created_at) // Show newest first
                    .ThenBy(p => p.name) // Then alphabetically
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (products == null || !products.Any())
                {
                    return new List<ChatbotProductViewModel>();
                }

                // Convert to view model with complete data
                var result = new List<ChatbotProductViewModel>();

                foreach (var product in products)
                {
                    // Use the existing discount_price from the model if available
                    // Otherwise, check for active discounts in the discount system
                    decimal? finalDiscountPrice = product.discount_price;

                    // If no direct discount price, check the discount system
                    if (!finalDiscountPrice.HasValue)
                    {
                        var activeDiscount = db.product_discounts
                            .Include(pd => pd.Discount)
                            .Where(pd => pd.product_id == product.id &&
                                        pd.Discount.is_active == true &&
                                        pd.Discount.start_date <= currentDate &&
                                        (pd.Discount.end_date == null || pd.Discount.end_date >= currentDate))
                            .FirstOrDefault();

                        if (activeDiscount != null && activeDiscount.Discount.value.HasValue && product.price.HasValue)
                        {
                            if (activeDiscount.Discount.discount_type == "Percentage")
                            {
                                finalDiscountPrice = product.price.Value * (1 - activeDiscount.Discount.value.Value / 100);
                            }
                            else if (activeDiscount.Discount.discount_type == "Fixed")
                            {
                                finalDiscountPrice = product.price.Value - activeDiscount.Discount.value.Value;
                            }

                            // Ensure discount price is not negative
                            if (finalDiscountPrice < 0)
                                finalDiscountPrice = 0;
                        }
                    }

                    var viewModel = new ChatbotProductViewModel
                    {
                        ProductId = product.id,
                        ProductName = product.name ?? "Product",
                        Brand = product.Brand ?? "Amazon",
                        Price = Math.Round(product.price ?? 0, 2),
                        DiscountPrice = finalDiscountPrice.HasValue ? Math.Round(finalDiscountPrice.Value, 2) : (decimal?)null,
                        ImageUrl = GetProductImageUrl(product.id),
                        Rating = GetProductRating(product.id),
                        ReviewCount = GetProductRatingCount(product.id),
                        Description = product.description,
                        TotalSold = GetProductSalesCount(product.id),
                        Prime = _random.Next(2) == 1, // Random for now, you can implement actual Prime logic
                        DeliveryTiming = DateTime.Now.AddDays(_random.Next(1, 4)),
                        StockQuantity = product.stock_quantity ?? 0,
                        Colors = product.Colors,
                        Sizes = product.Sizes,
                        Sku = product.sku,
                        CreatedAt = product.created_at ?? DateTime.Now,
                        IsActive = product.is_active ?? false,
                        CategoryId = product.category_id
                    };

                    result.Add(viewModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetProductsForChatbot: {ex.Message}");
                return new List<ChatbotProductViewModel>();
            }
        }
    }
}
