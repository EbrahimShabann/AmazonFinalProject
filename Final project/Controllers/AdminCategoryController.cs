using Final_project.Models;
using Final_project.Repository;
using Final_project.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Final_project.Controllers
{
    [Authorize(Roles = "admin")]

    public class AdminCategoryController : Controller
    {

        private readonly UnitOfWork unitOfWork;

        public AdminCategoryController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            int page = 1, pageSize = 10;
            var categories = unitOfWork.CategoryRepository.GetAll(c => !c.is_deleted).ToList();
            foreach (category category in categories)
            {
                category.CreatedByUser = await unitOfWork.UserRepository.GetByIdAsync(category.created_by);

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
        public async Task<IActionResult> ActivateCategory(string id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return Json(new { success = false, message = "Category not found" });
            foreach (product p in unitOfWork.ProductRepository.GetAll(p => p.category_id == id).ToList())
            {
                p.is_active = true;

            }
            category.is_active = true;
            unitOfWork.save();

            return Json(new { success = true, message = "Category activated successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateCategory(string id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return Json(new { success = false, message = "Category not found" });
            foreach (product p in unitOfWork.ProductRepository.GetAll(p => p.category_id == id).ToList())
            {
                p.is_active = false;

            }
            category.is_active = false;
            unitOfWork.save();

            return Json(new { success = true, message = "Category deactivated successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return Json(new { success = false, message = "Category not found" });
            foreach (product p in unitOfWork.ProductRepository.GetAll(p => p.category_id == id).ToList())
            {
                p.is_active = false;
                p.is_deleted = true;

            }
            category.is_deleted = true;
            category.is_active = false;
            category.deleted_by = User.FindFirstValue(ClaimTypes.NameIdentifier);
            unitOfWork.save();

            return Json(new { success = true, message = "Category deleted successfully" });
        }
        public async Task<IActionResult> CategoryListPartial()
        {
            var categories = unitOfWork.CategoryRepository.GetAll(c => !c.is_deleted).OrderByDescending(c => c.is_active).ToList();
            foreach (category category in categories)
            {
                category.CreatedByUser = await unitOfWork.UserRepository.GetByIdAsync(category.created_by);

            }
            return PartialView(categories);
        }

        [HttpGet]
        public async Task<IActionResult> FilterCategories(string name, string createdBy, string status, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 10)
        {
            var query = unitOfWork.CategoryRepository.GetAll(c => !c.is_deleted).ToList(); // executes the query now
            foreach (category category in query)
            {
                category.CreatedByUser = await unitOfWork.UserRepository.GetByIdAsync(category.created_by);
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
            var suggestions = unitOfWork.CategoryRepository.GetAll(c => c.name.Contains(term))
                .Select(c => c.name)
                .Distinct()
                .Take(5)
                .ToList();

            return Json(suggestions);
        }

        [HttpGet]
        public IActionResult SuggestCreators(string term)
        {
            var suggestions = unitOfWork.UserRepository.GetAll(u => u.UserName.Contains(term))
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
                model.image_url = "/Category/" + fileName;
            }


            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newCategory = new category
            {
                id = Guid.NewGuid().ToString(),
                name = model.name,
                description = model.description,
                parent_category_id = model.parent_category_id,
                image_url = model.image_url,
                created_by = currentUserId, // Replace with your logic
                created_at = DateTime.Now,
                ParentCategory = await unitOfWork.CategoryRepository.GetByIdAsync(model.parent_category_id),
                CreatedByUser = await unitOfWork.UserRepository.GetByIdAsync(currentUserId), // Replace with your logic
                last_modified_by = currentUserId, // Replace with your logic
                last_modified_at = DateTime.Now,
                LastModifiedByUser = await unitOfWork.UserRepository.GetByIdAsync(currentUserId), // Replace with your logic
                deleted_by = null, // Set to null if not deleted

            };

            unitOfWork.CategoryRepository.add(newCategory);
            unitOfWork.save();
            return Json(new { success = true, message = "Category added successfully!" });
        }

        public async Task<IActionResult> CategoryDetails(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category == null) return NotFound();

            var subcategoriesCount = unitOfWork.CategoryRepository.GetAll(c => c.parent_category_id == id).Count();
            var productCount = unitOfWork.ProductRepository.GetAll(p => p.category_id == id).Count();

            ViewBag.SubcategoryCount = subcategoriesCount;
            ViewBag.ProductCount = productCount;

            return View(category);
        }

    }
}