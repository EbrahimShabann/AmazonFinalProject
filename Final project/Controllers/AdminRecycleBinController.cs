using Final_project.Models;
using Final_project.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace Final_project.Controllers
{
    public class AdminRecycleBinController : Controller
    {
        private readonly AmazonDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminRecycleBinController(AmazonDBContext context, UserManager<ApplicationUser> _userManager)
        {
            _context = context;
            this._userManager = _userManager;

        }
        // GET: /AdminRecycleBin
        public async Task<IActionResult> IndexAsync()
        {
            var deletedCategories = _context.categories
                .Where(c => c.is_deleted)
                .ToList();

            var deletedProducts = _context.products
                .Where(p => p.is_deleted)
                .ToList();
            var DeletedSupport = (await _userManager.GetUsersInRoleAsync("Support"));
            var DeletedCustomerService = DeletedSupport.Where(c=>c.is_deleted).ToList();

            var DeletedSeller = (await _userManager.GetUsersInRoleAsync("Seller")).Where(c => c.is_deleted).ToList();
            var Users = _context.Users.ToList();

            foreach (var product in deletedProducts)
            {
                product.Seller = _context.Users.FirstOrDefault(u => u.Id == product.seller_id);
            }
            foreach (var category in deletedCategories)
            {
                category.CreatedByUser = _context.Users.FirstOrDefault(u => u.Id == category.created_by);
                category.ParentCategory = _context.categories.FirstOrDefault(c=>c.id== category.parent_category_id);
                category.DeletedByUser = _context.Users.FirstOrDefault(u => u.Id == category.deleted_by);
                category.LastModifiedByUser = _context.Users.FirstOrDefault(u=>u.Id == category.last_modified_by);
            }
            var viewModel = new RecycleBinViewModel
            {
                DeletedCategories = deletedCategories,
                DeletedProducts = deletedProducts,
                DeletedCustomerService = DeletedCustomerService,
                DeletedSellers = DeletedSeller,
                Users = Users

            };

            return View(viewModel);
        }

        // POST: Restore Category
        [HttpPost]
        public IActionResult RestoreCategory(string id)
        {
            var category = _context.categories.Find(id);
            if (category == null || !category.is_deleted)
                return Json(new { success = false, message = "Category not found." });
            var products = _context.products.Where(p => p.category_id == id);
            category.is_deleted = false;
            category.is_active = false;
            foreach(product p in products)
            {
                p.is_deleted = false;
                p.is_active = false;
                p.is_approved = true;
            }
            _context.SaveChanges();

            return Json(new { success = true, message = "Category restored successfully." });
        }

        // POST: Permanently Delete Category
        [HttpPost]
        public IActionResult HardDeleteCategory(string id)
        {
            var category = _context.categories.Find(id);
            if (category == null || !category.is_deleted)
                return Json(new { success = false, message = "Category not found." });
            var products = _context.products.Where(p => p.category_id == id);
            foreach (product p in products)
            {
                _context.products.Remove(p);
            }
            _context.categories.Remove(category);
            _context.SaveChanges();

            return Json(new { success = true, message = "Category deleted permanently." });
        }

        // POST: Restore Product
        [HttpPost]
        public IActionResult RestoreProduct(string id)
        {
            var product = _context.products.Find(id);
            if (product == null || !product.is_deleted)
                return Json(new { success = false, message = "Product not found." });

            product.is_deleted = false;
            product.is_active = false;
            product.is_approved = true;
            _context.SaveChanges();

            return Json(new { success = true, message = "Product restored successfully." });
        }

        // POST: Permanently Delete Product
        [HttpPost]
        public IActionResult HardDeleteProduct(string id)
        {
            var product = _context.products.Find(id);
            if (product == null || !product.is_deleted)
                return Json(new { success = false, message = "Product not found." });

            _context.products.Remove(product);
            _context.SaveChanges();

            return Json(new { success = true, message = "Product deleted permanently." });
        }


        // POST: Restore customer service
        [HttpPost]
        public IActionResult RestoreCastomerService(string id)
        {
            var CastomerService = _context.Users.Find(id);
            if (CastomerService == null || !CastomerService.is_deleted)
                return Json(new { success = false, message = "Customer Service not found." });

            CastomerService.is_deleted = false;
            CastomerService.is_active = false;
            _context.SaveChanges();

            return Json(new { success = true, message = "Customer Service restored successfully." });
        }

        // POST: Permanently Delete customer service
        [HttpPost]
        public IActionResult HardDeleteCustomerService(string id)
        {
            var CastomerService = _context.Users.Find(id);
            if (CastomerService == null || !CastomerService.is_deleted)
                return Json(new { success = false, message = "Customer Service not found." });

            _context.Users.Remove(CastomerService);
            _context.SaveChanges();

            return Json(new { success = true, message = "Customer Service deleted permanently." });
        }

        // POST: Restore seller
        [HttpPost]
        public IActionResult RestoreSeller(string id)
        {
            var seller = _context.Users.Find(id);
            if (seller == null || !seller.is_deleted)
                return Json(new { success = false, message = "Seller not found." });

            var products = _context.products.Where(p => p.category_id == id);
            
            foreach (product p in products)
            {
                p.is_deleted = false;
                p.is_active = false;
                p.is_approved = true;
            }


            seller.is_deleted = false;
            seller.is_active = false;
            _context.SaveChanges();

            return Json(new { success = true, message = "Seller restored successfully." });
        }

        // POST: Permanently Delete seller
        [HttpPost]
        public IActionResult HardDeleteSeller(string id)
        {
            var seller = _context.Users.Find(id);
            if (seller == null || !seller.is_deleted)
                return Json(new { success = false, message = "Seller not found." });
            var products = _context.products.Where(p => p.category_id == id);
            foreach (product p in products)
            {
                _context.products.Remove(p);
            }
            _context.Users.Remove(seller);
            _context.SaveChanges();
            return Json(new { success = true, message = "Seller deleted permanently." });
        }
    }
}
