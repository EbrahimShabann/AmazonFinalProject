using Final_project.Models;
using Final_project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    [Authorize(Roles = "admin,superadmin")]

    public class AdminProductsController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminProductsController(UnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager)
        {
            this.unitOfWork = unitOfWork;
            this._userManager = _userManager;
        }

        public async Task<IActionResult> pendingProduct()
        {
            var CountPendingProducts = unitOfWork.ProductRepository.GetAll(p => (bool)!p.is_approved && (bool)p.is_active && !p.is_deleted).Count();
            var CountAcceptedProducts = unitOfWork.ProductRepository.GetAll(p => (bool)p.is_approved && (bool)p.is_active).Count();
            var CountRegectedProducts = unitOfWork.ProductRepository.GetAll(p => (bool)!p.is_approved && (bool)!p.is_active && !p.is_deleted).Count();
            var PendingProducts = unitOfWork.ProductRepository.GetAll(p => (bool)!p.is_approved && (bool)p.is_active).OrderByDescending(t => t.created_at).ToList();
            List<category> category = unitOfWork.CategoryRepository.GetAll().ToList();
            foreach (product p in PendingProducts)
            {
                p.category = category.FirstOrDefault(c => c.id == p.category_id);
                p.Seller = await unitOfWork.UserRepository.GetByIdAsync(p.seller_id);
                p.product_images = unitOfWork.ProductImageRepository.GetAll(img => img.product_id == p.id).ToList();
            }

            ViewBag.CountPendingProducts = CountPendingProducts;
            ViewBag.CountAcceptedProducts = CountAcceptedProducts;
            ViewBag.CountRegectedProducts = CountRegectedProducts;
            ViewBag.PendeingProducts = PendingProducts;
            ViewBag.ActivePage = "Products";

            return View();
        }
        public async Task<IActionResult> AllProductsAsync()
        {
            var CountActiveProducts = unitOfWork.ProductRepository.GetAll(p => (bool)p.is_approved && (bool)p.is_active && !p.is_deleted).Count();
            var CountInactiveProducts = unitOfWork.ProductRepository.GetAll(p => (bool)p.is_approved && (bool)!p.is_active && !p.is_deleted).Count();
            var CountDeletedProducts = unitOfWork.ProductRepository.GetAll(p => p.is_deleted).Count();
            ViewBag.CountActiveProducts = CountActiveProducts;
            ViewBag.CountInactiveProducts = CountInactiveProducts;
            ViewBag.CountDeletedProducts = CountDeletedProducts;
            var Sellers = (await _userManager.GetUsersInRoleAsync("Seller")).ToList();
            var ActiveSellers = Sellers.Where(u => u.is_active).Select(ur => ur.Id)
                .Distinct()
                .ToList();
            var categories = unitOfWork.CategoryRepository.GetAll().ToList();
            ViewBag.Categories = categories;
            List<product> products = unitOfWork.ProductRepository.GetAll(p => !p.is_deleted)
                .OrderByDescending(p => !p.is_approved & p.is_active & !p.is_deleted).ThenByDescending(p => p.is_approved & p.is_active & !p.is_deleted).ThenByDescending(p => p.is_approved & !p.is_active & !p.is_deleted)
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
            var suggestions = unitOfWork.ProductRepository.GetAll(p => p.name.Contains(term))
                .Select(p => new { name = p.name })
                .Distinct()
                .ToList();

            return Json(suggestions);
        }
        [HttpGet]
        public JsonResult GetSellerSuggestions(string term)
        {
            var sellers = unitOfWork.ProductRepository.GetAll(p => p.Seller.UserName.Contains(term))
                .Select(p => new { name = p.Seller.UserName })
                .Distinct()
                .ToList();

            return Json(sellers);
        }
        [HttpGet]
        public JsonResult FilterProducts(string name, string seller, string? categoryId, string status, bool approvedByMe, DateTime? approvedFrom, DateTime? approvedTo, int page = 1, int pageSize = 10)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = unitOfWork.ProductRepository.GetAll(p => !p.is_deleted)
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
                    pending = (p.is_active & !p.is_approved & !p.is_deleted),
                    approved = (p.is_active & p.is_approved & !p.is_deleted),
                    rejected = (!p.is_active & !p.is_approved & !p.is_deleted),
                    inactive = (!p.is_active & p.is_approved & !p.is_deleted),
                }).ToList();
            return Json(new { data = data, totalPages = totalPages });
        }
        [HttpPost]
        public async Task<JsonResult> ApproveProduct(string id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) return Json(new { success = false });

            product.is_approved = true;
            product.is_active = true;
            product.is_deleted = false;
            product.approved_at = DateTime.Now;
            product.approved_by = User.FindFirstValue(ClaimTypes.NameIdentifier);
            unitOfWork.save();

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<JsonResult> RejectProduct(string id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) return Json(new { success = false });

            product.is_approved = false;
            product.is_active = false;
            product.is_deleted = false;
            unitOfWork.save();

            return Json(new { success = true });
        }
        public async Task<JsonResult> deleteProduct(string id)
        {
            var Product = await unitOfWork.ProductRepository.GetByIdAsync(id);
            if (Product == null) return Json(new { success = false });

            Product.is_approved = false;
            Product.is_active = false;
            Product.is_deleted = true;

            unitOfWork.save();

            return Json(new { success = true });
        }
        public async Task<JsonResult> activeProduct(string id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) return Json(new { success = false });

            product.is_approved = true;
            product.is_active = true;
            product.is_deleted = false;
            unitOfWork.save();

            return Json(new { success = true });
        }
        public async Task<JsonResult> inactiveProduct(string id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) return Json(new { success = false });

            product.is_approved = true;
            product.is_active = false;
            product.is_deleted = false;
            unitOfWork.save();

            return Json(new { success = true });
        }


    }
}
