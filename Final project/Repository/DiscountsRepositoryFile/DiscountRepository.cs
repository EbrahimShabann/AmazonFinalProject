using Final_project.Models;
using Humanizer;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AmazonDBContext _context;
        public DiscountRepository(AmazonDBContext context) { _context = context; }

        public IQueryable<discount> GetAll(Expression<Func<discount, bool>> filter = null, params Expression<Func<discount, object>>[] includes)
        {
            IQueryable<discount> query = _context.discounts;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }

        public async Task<discount> GetAsync(Expression<Func<discount, bool>> filter, params Expression<Func<discount, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }

        public async Task<discount> GetByIdAsync(string id) => await _context.discounts.FindAsync(id);

        public async Task<int> GetCountAsync(Expression<Func<discount, bool>> filter = null)
        {
            if (filter != null)
                return await _context.discounts.CountAsync(filter);
            return await _context.discounts.CountAsync();
        }

        public async Task AddAsync(discount entity) => await _context.discounts.AddAsync(entity);
        public void add(discount entity) => _context.discounts.Add(entity);
  
        public void Update(discount entity) => _context.discounts.Update(entity);
        public void Delete(discount entity) => _context.discounts.Remove(entity);

        public List<discount> getAll()
        {
            throw new NotImplementedException();
        }

        public discount getById(string id)
        {
            throw new NotImplementedException();
        }


        private decimal CalculateDiscountedPrice(decimal originalPrice, discount discount)
        {
            if (!discount.value.HasValue) return originalPrice;

            decimal discountValue = discount.value.Value;

            // Apply discount based on type
            switch (discount.discount_type?.ToLower())
            {
                case "percentage":
                    // For percentage discount (e.g., 20% off)
                    return originalPrice - (originalPrice * discountValue / 100);

                case "fixed":
                case "amount":
                    // For fixed amount discount (e.g., $10 off)
                    return Math.Max(0, originalPrice - discountValue);

                default:
                    // Default to percentage if type is not specified
                    return originalPrice - (originalPrice * discountValue / 100);
            }
        }
     //   / Method to apply discount to multiple products
        public void ApplyDiscountToProducts(discount entity, List<string> productIds)
        {
            _context.discounts.Add(entity);

            foreach (string productId in productIds)
            {
                // Create product-discount relationship
                var productDiscount = new product_discount
                {
                    id = Guid.NewGuid().ToString(),
                    product_id = productId,
                    discount_id = entity.id
                };
                _context.product_discounts.Add(productDiscount);

                // Get and update product
                var product = _context.products.FirstOrDefault(p => p.id == productId);
                if (product != null && product.price.HasValue)
                {
                    decimal discountedPrice = CalculateDiscountedPrice(product.price.Value, entity);
                    product.discount_price = discountedPrice;
                    product.last_modified_at = DateTime.UtcNow;
                }
            }

            _context.SaveChanges();
        }


    }
} 