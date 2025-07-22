using Final_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Controllers
{
    public class AdminProductsController : Controller
    {
        private readonly AmazonDBContext _context;

        public AdminProductsController(AmazonDBContext context)
        {
            _context = context;
        }

        public IActionResult pendingProduct()
        {
            var CountPendingProducts = _context.products.Count(p => (bool)!p.is_approved && (bool)p.is_active);
            var CountAcceptedProducts = _context.products.Count(p => (bool)p.is_approved);
            var CountRegectedProducts = _context.products.Count(p => (bool)!p.is_approved && (bool)!p.is_active);
            var PendingProducts = _context.products.Where(p => (bool)!p.is_approved && (bool)p.is_active).OrderByDescending(t => t.created_at).ToList();
            List<product_image> ProductImages = _context.product_images.ToList();
            List<ApplicationUser> seller = _context.Users.ToList();
            List<category> category = _context.categories.ToList();
            foreach (product p in PendingProducts)
            {
                p.category = category.FirstOrDefault(c => c.id == p.category_id);
                p.Seller = _context.Users.FirstOrDefault(u=>u.Id==p.seller_id);
                p.Product_Images = _context.product_images.Where(img=>img.product_id==p.id).ToList();
            }
            //var productList = _context.products
            //.Where(p => (bool)p.is_active)
            //.OrderBy(p => p.is_approved)
            //.ThenByDescending(p => p.created_at)
            //.ToList();
            ViewBag.CountPendingProducts = CountPendingProducts;
            ViewBag.CountAcceptedProducts = CountAcceptedProducts;
            ViewBag.CountRegectedProducts = CountRegectedProducts;
            ViewBag.PendeingProducts = PendingProducts;

            //ViewBag.Categories = _context.categories.ToList();

            return View();
        }
        public IActionResult AllProducts()
        {
            var sellerRoleId = _context.Roles.Where(r => r.Name == "Seller").Select(r => r.Id).FirstOrDefault();
            var Sellers = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId) && u.is_active).ToList();
            var ActiveSellers = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId) && u.is_active).Select(ur => ur.Id)
                .Distinct()
                .ToList();
            var categories = _context.categories.ToList();

            List<product> products = _context.products
                .Where(p => p.is_active == true && ActiveSellers.Contains(p.seller_id))
                .OrderByDescending(p => !p.is_approved) // Pending products first
                .ThenByDescending(p => p.created_at)   // Newer first
                .ToList();
            foreach (product p in products) {
                p.Seller = Sellers.FirstOrDefault(s => s.Id == p.seller_id);
                p.category = categories.FirstOrDefault(c => c.id == p.category_id);
            }
            return View(products);
        }
        [HttpGet("GetFiltered")]
        public async Task<IActionResult> GetFiltered(
        string name, string seller, string category,
        decimal? minPrice, decimal? maxPrice, string status)
        {
            var products = _context.products
                .Include(p => p.category)
                .Include(p => p.Seller)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                products = products.Where(p => p.name.Contains(name));

            if (!string.IsNullOrWhiteSpace(seller))
                products = products.Where(p => p.Seller.UserName.Contains(seller));

            if (!string.IsNullOrWhiteSpace(category))
                products = products.Where(p => p.category.name.Contains(category));

            if (minPrice.HasValue)
                products = products.Where(p => p.price >= minPrice);

            if (maxPrice.HasValue)
                products = products.Where(p => p.price <= maxPrice);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "approved")
                    products = products.Where(p => p.is_approved == true);
                else if (status == "pending")
                    products = products.Where(p => p.is_approved == false && p.is_active == true);
                else if (status == "rejected")
                    products = products.Where(p => p.is_approved == false && p.is_active == false);
            }

            var result = await products.Select(p => new
            {
                id = p.id,
                name = p.name,
                price = p.price,
                sellerName = p.Seller.UserName,
                categoryName = p.category.name,
                isApproved = p.is_approved,
                isActive = p.is_active
            }).ToListAsync();

            return Json(result);
        }

        [HttpPost("ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var product = await _context.products.FindAsync(id);
            if (product == null) return NotFound();

            product.is_active = !product.is_active;
            _context.Update(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Approve/{id}")]
        public async Task<IActionResult> Approve(string id)
        {
            var product = await _context.products.FindAsync(id);
            if (product == null) return NotFound();

            product.is_approved = true;
            product.approved_at = DateTime.UtcNow;
            product.approved_by = User.Identity.Name;
            _context.Update(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Reject/{id}")]
        public async Task<IActionResult> Reject(string id)
        {
            var product = await _context.products.FindAsync(id);
            if (product == null) return NotFound();

            product.is_approved = false;
            product.is_active = false;
            _context.Update(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Optional: Populate filter dropdowns
        [HttpGet("GetFiltersData")]
        public async Task<IActionResult> GetFiltersData()
        {
            var sellers = await _context.Users.Select(u => u.UserName).Distinct().ToListAsync();
            var categories = await _context.categories.Select(c => c.name).Distinct().ToListAsync();
            return Json(new { sellers, categories });
        }
    }
}


