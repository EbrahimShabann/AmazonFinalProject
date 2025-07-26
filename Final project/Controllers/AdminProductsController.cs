using Final_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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
            var CountPendingProducts = _context.products.Count(p => (bool)!p.is_approved && (bool)p.is_active && !p.is_deleted);
            var CountAcceptedProducts = _context.products.Count(p => (bool)p.is_approved && (bool)p.is_active);
            var CountRegectedProducts = _context.products.Count(p => (bool)!p.is_approved && (bool)!p.is_active && !p.is_deleted);
            var PendingProducts = _context.products.Where(p => (bool)!p.is_approved && (bool)p.is_active).OrderByDescending(t => t.created_at).ToList();
            List<product_image> ProductImages = _context.product_images.ToList();
            List<ApplicationUser> seller = _context.Users.ToList();
            List<category> category = _context.categories.ToList();
            foreach (product p in PendingProducts)
            {
                p.category = category.FirstOrDefault(c => c.id == p.category_id);
                p.Seller = _context.Users.FirstOrDefault(u => u.Id == p.seller_id);
                p.product_images = _context.product_images.Where(img => img.product_id == p.id).ToList();
            }

            ViewBag.CountPendingProducts = CountPendingProducts;
            ViewBag.CountAcceptedProducts = CountAcceptedProducts;
            ViewBag.CountRegectedProducts = CountRegectedProducts;
            ViewBag.PendeingProducts = PendingProducts;
            ViewBag.ActivePage = "Products";

            return View();
        }
        public IActionResult AllProducts()
        {
            var CountActiveProducts = _context.products.Count(p => (bool)p.is_approved && (bool)p.is_active && !p.is_deleted);
            var CountInactiveProducts = _context.products.Count(p => (bool)p.is_approved && (bool)!p.is_active && !p.is_deleted);
            var CountDeletedProducts = _context.products.Count(p => p.is_deleted);
            ViewBag.CountActiveProducts = CountActiveProducts;
            ViewBag.CountInactiveProducts = CountInactiveProducts;
            ViewBag.CountDeletedProducts = CountDeletedProducts;
            var sellerRoleId = _context.Roles.Where(r => r.Name == "Seller").Select(r => r.Id).FirstOrDefault();
            var Sellers = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId) && u.is_active).ToList();
            var ActiveSellers = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRoleId) && u.is_active).Select(ur => ur.Id)
                .Distinct()
                .ToList();
            var categories = _context.categories.ToList();
            ViewBag.Categories = categories;
            List<product> products = _context.products.Where(p => !p.is_deleted)
                .OrderByDescending(p => !p.is_approved& p.is_active& !p.is_deleted).ThenByDescending(p=> p.is_approved & p.is_active & !p.is_deleted).ThenByDescending(p=> p.is_approved & !p.is_active & !p.is_deleted)
                .ThenByDescending(p => p.created_at) 
                .ToList();
            foreach (product p in products)
            {
                p.Seller = Sellers.FirstOrDefault(s => s.Id == p.seller_id);
                p.category = categories.FirstOrDefault(c => c.id == p.category_id);
            }
            ViewBag.ActivePage = "Products";

            return View(products);
        }
        public JsonResult GetSuggestions(string term)
        {
            var suggestions = _context.products
                .Where(p => p.name.Contains(term))
                .Select(p => new { name = p.name })
                .Distinct()
                .ToList();

            return Json(suggestions);
        }
        [HttpGet]
        public JsonResult GetSellerSuggestions(string term)
        {
            var sellers = _context.products
                .Where(p => p.Seller.UserName.Contains(term))
                .Select(p => new { name = p.Seller.UserName })
                .Distinct()
                .ToList();

            return Json(sellers);
        }
        [HttpGet]
        public JsonResult FilterProducts(string name, string seller, string? categoryId, string status, bool approvedByMe, DateTime? approvedFrom, DateTime? approvedTo, int page = 1, int pageSize = 10)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = _context.products.Where(p => !p.is_deleted)
                .OrderByDescending(p => !p.is_approved & p.is_active & !p.is_deleted).ThenByDescending(p => p.is_approved & p.is_active & !p.is_deleted).ThenByDescending(p => p.is_approved & !p.is_active & !p.is_deleted)
                .ThenByDescending(p => p.created_at)   
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                products = products.Where(p => p.name.Contains(name));

            if (!string.IsNullOrWhiteSpace(seller))
                products = products.Where(p => p.Seller.UserName.Contains(seller));

            if (!string.IsNullOrEmpty(categoryId))
                products = products.Where(p => p.category_id == categoryId);

            if (approvedByMe)
                products = products.Where(p => p.approved_by == currentUserId);

            if (approvedFrom.HasValue)
                products = products.Where(p => p.approved_at >= approvedFrom.Value.Date);
            if (approvedTo.HasValue)
                products = products.Where(p => p.approved_at <= approvedTo.Value.Date);
            if (!string.IsNullOrWhiteSpace(status))
            {
                switch (status.ToLower())
                {
                    case "approved":
                        products = products.Where(p => (bool)p.is_active & (bool)p.is_approved & !p.is_deleted);
                        break;
                    case "pending":
                        products = products.Where(p => (bool)p.is_active & (bool)!p.is_approved & !p.is_deleted);
                        break;
                    case "rejected":
                        products = products.Where(p => (bool)!p.is_active & (bool)!p.is_approved & !p.is_deleted);
                        break;
                    case "inactive":
                        products = products.Where(p => (bool)!p.is_active & (bool)p.is_approved & !p.is_deleted);
                        break;
                }
            }
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    id = p.id,
                    name = p.name,
                    price = p.price,
                    sellerName = p.Seller.UserName,
                    categoryName = p.category.name,
                    pending = (p.is_active&!p.is_approved&!p.is_deleted),
                    approved = (p.is_active & p.is_approved & !p.is_deleted),
                    rejected = (!p.is_active & !p.is_approved & !p.is_deleted),
                    inactive = (!p.is_active & p.is_approved & !p.is_deleted),
                }).ToList();
            return Json(new {data=data,totalPages = totalPages });
        }
        [HttpPost]
        public JsonResult ApproveProduct(string id)
        {
            var product = _context.products.FirstOrDefault(p => p.id == id);
            if (product == null) return Json(new { success = false });

            product.is_approved = true;
            product.is_active = true;
            product.is_deleted = false;
            product.approved_at = DateTime.Now;
            product.approved_by = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        [HttpPost]
        public JsonResult RejectProduct(string id)
        {
            var product = _context.products.FirstOrDefault(p => p.id == id);
            if (product == null) return Json(new { success = false });

            product.is_approved = false;
            product.is_active = false;
            product.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }
        public JsonResult deleteProduct(string id)
        {
            var product = _context.products.FirstOrDefault(p => p.id == id);
            if (product == null) return Json(new { success = false });

            product.is_approved = false;
            product.is_active = false;
            product.is_deleted = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }
        public JsonResult activeProduct(string id)
        {
            var product = _context.products.FirstOrDefault(p => p.id == id);
            if (product == null) return Json(new { success = false });

            product.is_approved = true;
            product.is_active = true;
            product.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }
        public JsonResult inactiveProduct(string id)
        {
            var product = _context.products.FirstOrDefault(p => p.id == id);
            if (product == null) return Json(new { success = false });

            product.is_approved = true;
            product.is_active = false;
            product.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }


    }
}



