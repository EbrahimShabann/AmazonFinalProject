using Final_project.Models;
using Final_project.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Controllers
{
    public class AdminRecycleBinController : Controller
    {
        private readonly AmazonDBContext _context;

        public AdminRecycleBinController(AmazonDBContext context)
        {
            _context = context;
        }
        // GET: /AdminRecycleBin
        public IActionResult Index()
        {
            var deletedCategories = _context.categories
                .Include(c => c.CreatedByUser)
                .Where(c => c.is_deleted)
                .ToList();

            var deletedProducts = _context.products
                .Include(p => p.Seller)
                .Where(p => p.is_deleted)
                .ToList();
            foreach (var product in deletedProducts)
            {
                product.Seller = _context.Users.FirstOrDefault(u => u.Id == product.seller_id);
            }
            foreach (var category in deletedCategories)
            {
                category.CreatedByUser = _context.Users.FirstOrDefault(u => u.Id == category.created_by);
            }
            var viewModel = new RecycleBinViewModel
            {
                DeletedCategories = deletedCategories,
                DeletedProducts = deletedProducts
            };

            return View(viewModel);
        }

        // POST: Restore Category
        [HttpPost]
        public IActionResult RestoreCategory(int id)
        {
            var category = _context.categories.Find(id);
            if (category == null || !category.is_deleted)
                return Json(new { success = false, message = "Category not found." });

            category.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true, message = "Category restored successfully." });
        }

        // POST: Permanently Delete Category
        [HttpPost]
        public IActionResult HardDeleteCategory(int id)
        {
            var category = _context.categories.Find(id);
            if (category == null || !category.is_deleted)
                return Json(new { success = false, message = "Category not found." });

            _context.categories.Remove(category);
            _context.SaveChanges();

            return Json(new { success = true, message = "Category deleted permanently." });
        }

        // POST: Restore Product
        [HttpPost]
        public IActionResult RestoreProduct(int id)
        {
            var product = _context.products.Find(id);
            if (product == null || !product.is_deleted)
                return Json(new { success = false, message = "Product not found." });

            product.is_deleted = false;
            _context.SaveChanges();

            return Json(new { success = true, message = "Product restored successfully." });
        }

        // POST: Permanently Delete Product
        [HttpPost]
        public IActionResult HardDeleteProduct(int id)
        {
            var product = _context.products.Find(id);
            if (product == null || !product.is_deleted)
                return Json(new { success = false, message = "Product not found." });

            _context.products.Remove(product);
            _context.SaveChanges();

            return Json(new { success = true, message = "Product deleted permanently." });
        }
    }
}
