using Final_project.Models;
using Final_project.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Final_project.Controllers
{
    public class AdminCategoryController : Controller
    {

        private readonly AmazonDBContext _context;

        public AdminCategoryController(AmazonDBContext context)
        {
            _context = context;
            
        }

        // GET: /Category
        public IActionResult Index()
        {
            int page = 1, pageSize = 10;
            var categories = _context.categories.Where(c => !c.is_deleted).ToList();
            foreach (category category in categories)
            {
                category.CreatedByUser = _context.Users.FirstOrDefault(u => u.Id == category.created_by);

            }
            
            var totalCount = categories.Count;
            var paginated = categories
            .OrderByDescending(c => c.is_active)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

                    ViewBag.TotalCount = totalCount;
                    ViewBag.Page = page;
                    ViewBag.PageSize = pageSize;
            return View(categories);
        }
        [HttpPost]
        public IActionResult ActivateCategory(string id)
        {
            var category = _context.categories.FirstOrDefault(c => c.id == id);
            if (category == null)
                return Json(new { success = false, message = "Category not found" });
            foreach (product p in _context.products.Where(p=>p.category_id==id).ToList())
            {
                p.is_active= true;
               
            }
            category.is_active = true;
            _context.SaveChanges();

            return Json(new { success = true, message = "Category activated successfully" });
        }

        [HttpPost]
        public IActionResult DeactivateCategory(string id)
        {
            var category = _context.categories.FirstOrDefault(c => c.id == id);
            if (category == null)
                return Json(new { success = false, message = "Category not found" });
            foreach (product p in _context.products.Where(p => p.category_id == id).ToList())
            {
                p.is_active = false;

            }
            category.is_active = false;
            _context.SaveChanges();

            return Json(new { success = true, message = "Category deactivated successfully" });
        }

        [HttpPost]
        public IActionResult DeleteCategory(string id)
        {
            var category = _context.categories.FirstOrDefault(c => c.id == id);
            if (category == null)
                return Json(new { success = false, message = "Category not found" });
            foreach (product p in _context.products.Where(p => p.category_id == id).ToList())
            {
                p.is_active = false;
                p.is_deleted = true;

            }
            category.is_deleted = true;
            category.is_active = false;
            _context.SaveChanges();

            return Json(new { success = true, message = "Category deleted successfully" });
        }
        public IActionResult CategoryListPartial()
        {
            var categories = _context.categories
                .Where(c => !c.is_deleted).OrderByDescending(c=> c.is_active)
                .ToList();
            foreach (category category in categories)
            {
                category.CreatedByUser = _context.Users.FirstOrDefault(u => u.Id == category.created_by);

            }
            return PartialView(categories);
        }

        [HttpGet]
        public IActionResult FilterCategories(string name, string createdBy, string status, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 10)
        {
            var query = _context.categories.Where(c=>!c.is_deleted).ToList(); // executes the query now
            foreach (category category in query)
            {
                category.CreatedByUser = _context.Users.FirstOrDefault(u => u.Id == category.created_by);
            }
            if (!string.IsNullOrEmpty(name))
                query = query.Where(c => c.name.Contains(name)).ToList();

            if (!string.IsNullOrEmpty(createdBy))
                query = query.Where(c => c.CreatedByUser != null && c.CreatedByUser.UserName.Contains(createdBy)).ToList();

            if (!string.IsNullOrEmpty(status))
            {
                switch (status.ToLower())
                {
                    case "active":
                        query = query.Where(c => (bool)c.is_active && !c.is_deleted).ToList();
                        break;
                    case "inactive":
                        query = query.Where(c => (bool)!c.is_active && !c.is_deleted).ToList();
                        break;
                    case "deleted":
                        query = query.Where(c => c.is_deleted).ToList();
                        break;
                }
            }

            if (dateFrom.HasValue)
                query = query.Where(c => c.created_at >= dateFrom.Value).ToList();

            if (dateTo.HasValue)
                query = query.Where(c => c.created_at <= dateTo.Value).ToList();

            int totalCount = query.Count();

            var paginated = query
                .OrderByDescending(c => c.is_active)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return PartialView("CategoryListPartial", paginated);
        }

        [HttpGet]
        public IActionResult SuggestCategoryNames(string term)
        {
            var suggestions = _context.categories
                .Where(c => c.name.Contains(term))
                .Select(c => c.name)
                .Distinct()
                .Take(5)
                .ToList();

            return Json(suggestions);
        }

        [HttpGet]
        public IActionResult SuggestCreators(string term)
        {
            var suggestions = _context.Users
                .Where(u => u.UserName.Contains(term))
                .Select(u => u.UserName)
                .Distinct()
                .Take(5)
                .ToList();

            return Json(suggestions);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(CategoryCreateViewModel model, IFormFile imgFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // For AJAX
            }

            if (imgFile != null && imgFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploads);

                var fileName = Path.GetFileName(imgFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await imgFile.CopyToAsync(stream);

                // Point your model at the saved path
                model.image_url = "/uploads/" + fileName;
            }



            var newCategory = new category
            {
                id= Guid.NewGuid().ToString(),
                name = model.name,
                description = model.description,
                parent_category_id = model.parent_category_id,
                image_url = model.image_url,
                created_by = _context.Users.FirstOrDefault(u=>u.UserName=="Admin1").Id, // Replace with your logic
                created_at = DateTime.Now,
                ParentCategory = _context.categories.FirstOrDefault(c => c.id == model.parent_category_id),
                CreatedByUser = _context.Users.FirstOrDefault(u => u.UserName == "Admin1"), // Replace with your logic
                last_modified_by = _context.Users.FirstOrDefault(u => u.UserName == "Admin1").Id, // Replace with your logic
                last_modified_at = DateTime.Now,
                LastModifiedByUser= _context.Users.FirstOrDefault(u => u.UserName == "Admin1"), // Replace with your logic
                deleted_by = null, // Set to null if not deleted

            };

            _context.categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Category added successfully!" });
        }

        public async Task<IActionResult> CategoryDetails(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var category = await _context.categories
                .Include(c => c.ParentCategory)
                .Include(c => c.CreatedByUser)
                .Include(c => c.LastModifiedByUser)
                .Include(c => c.DeletedByUser)
                .FirstOrDefaultAsync(c => c.id == id);

            if (category == null) return NotFound();

            var subcategoriesCount = await _context.categories.CountAsync(c => c.parent_category_id == id);
            var productCount = await _context.products.CountAsync(p => p.category_id == id);

            ViewBag.SubcategoryCount = subcategoriesCount;
            ViewBag.ProductCount = productCount;

            return View(category);
        }

    }
}