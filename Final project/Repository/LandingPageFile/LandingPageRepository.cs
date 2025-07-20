using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Final_project.Models;
using Final_project.ViewModel.LandingPageViewModels;
using Final_project.ViewModel.NewFolder;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.NewFolder
{
    public class LandingPageRepository : ILandingPageRepository
    {
        private readonly AmazonDBContext db;

        public LandingPageRepository(AmazonDBContext db)
        {
            this.db = db;
        }
        public List<LandingPageProducts> GetBestSellers(int Number = 10)
        {
            // Get top products from database
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
                .Take(Number)
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
                    delaviryTiming = DateTime.Now.AddDays(2),
                    prime = true
                };
                data.rattingStarMinuse = 5 - data.ratting;
                landingPageProducts.Add(data);
            }

            return landingPageProducts;
        }

        public List<LandingPageProducts> GetNewArrivals(int Number = 10)
        {
            // Get newest products from database
            var newProducts = db.products
                .Where(p => p.is_active == true &&
                           p.is_approved == true &&
                           p.is_deleted == false)
                .OrderByDescending(p => p.created_at)
                .Take(Number)
                .ToList();

            // Process each product
            var landingPageProducts = new List<LandingPageProducts>();

            foreach (var product in newProducts)
            {
                var data =new LandingPageProducts
                {
                    ProductId = product.id,
                    ProductName = product.name,
                    Price = product.price,
                    ImageUrl = GetProductImageUrl(product.id),
                    ratting = GetProductRating(product.id),
                    ratingCount = GetProductRatingCount(product.id),
                    delaviryTiming = DateTime.Now.AddDays(2), // Example value
                    prime = true // Example value
                };
                data.rattingStarMinuse = 5 - data.ratting;
                landingPageProducts.Add(data);
            }

            return landingPageProducts;
        }

        public List<LandingPageProductDiscount> GetNewDiscounts(int Number = 10)
        {
            var currentDate = DateTime.UtcNow;

            // Get newest discounted products with their discount information
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
                .Take(Number)
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
                    discountedPrice =item.Product.price - item.Discount.value.Value;
                }

                var data =new LandingPageProductDiscount
                {
                    ProductId = item.Product.id,
                    ProductName = item.Product.name,
                    ImageUrl = GetProductImageUrl(item.Product.id),
                    Price = Math.Round((decimal)item.Product.price,2),
                    DiscountPrice = discountedPrice,
                    TotalSold = (int)item.TotalSold,
                    ratting = GetProductRating(item.Product.id),
                    ratingCount = GetProductRatingCount(item.Product.id),
                    delaviryTiming = DateTime.Now.AddDays(2), // Example value
                    prime = true // Example value
                };
                data.rattingStarMinuse = 5 - data.ratting;
                result.Add(data);
            }

            return result;
        }

        public string GetProductImageUrl(string productId)
        {
            try
            {
                return db.product_images
                    .Where(pi => pi.product_id == productId && pi.is_primary == true)
                    .Select(pi => pi.image_url)
                    .FirstOrDefault() ?? "default-image-url.jpg";
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

                // First, get the products from database without calling GetProductImageUrl
                var products = db.products
                    .Where(p => p.is_active == true &&
                               p.is_approved == true &&
                               p.is_deleted == false &&
                               (p.name.Contains(searchTerm) || p.description.Contains(searchTerm)))
                    .OrderBy(p => p.name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new {
                        Id = p.id,
                        Name = p.name
                    })
                    .ToList(); // Execute the query here

                // Check if products is null before calling Select
                if (products == null)
                {
                    return new List<ProductSearchViewModel>();
                }

                // Then, create the view model with the method call
                return products.Select(p => new ProductSearchViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ImageUrl = GetProductImageUrl(p.Id) // Now this runs in memory, not in SQL
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (replace with your logging mechanism)
                // Logger.LogError(ex, "Error in ProductSearch method");

                // Return empty list instead of throwing
                return new List<ProductSearchViewModel>();
            }
        }



    }
}
