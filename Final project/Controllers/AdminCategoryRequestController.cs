using Final_project.Models;
using Final_project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "admin,superadmin")]
public class AdminCategoryRequestController : Controller
{
    private readonly UnitOfWork unitOfWork;

    public AdminCategoryRequestController(UnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    // GET: All pending, non-deleted requests
    public async Task<IActionResult> Index()
    {
        var requests = unitOfWork.CategoryRequestRepository.getAll()
            .Where(r => r.Status == "pending" && !r.isDeleted).ToList();
        return View(requests);
    }

    // POST: Approve request
    [HttpPost]
    public async Task<IActionResult> Approve(string id)
    {
        var request = unitOfWork.CategoryRequestRepository.getById(id);
        if (request == null || request.isDeleted) return NotFound();
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var newCategory = new category
        {
            id = Guid.NewGuid().ToString(),
            name = request.CategoryName,
            description = request.CategoryDiscription,
            parent_category_id = null,
            image_url = "category.jpg",
            created_by = currentUserId, // Replace with your logic
            created_at = DateTime.Now,
            ParentCategory =  null,
            CreatedByUser = await unitOfWork.UserRepository.GetByIdAsync(currentUserId), // Replace with your logic
            last_modified_by = currentUserId, // Replace with your logic
            last_modified_at = DateTime.Now,
            LastModifiedByUser = await unitOfWork.UserRepository.GetByIdAsync(currentUserId), // Replace with your logic
            deleted_by = null, // Set to null if not deleted
        };

        request.Status = "approved";
        unitOfWork.CategoryRepository.add(newCategory);
        unitOfWork.CategoryRequestRepository.Update(request);
        unitOfWork.save();
        return Ok();
    }

    // POST: Reject request
    [HttpPost]
    public async Task<IActionResult> Reject(string id)
    {
        var request =unitOfWork.CategoryRequestRepository.getById(id);
        if (request == null || request.isDeleted) return NotFound();

        request.Status = "rejected";
        request.isDeleted = true;
        unitOfWork.CategoryRequestRepository.Update(request);
        unitOfWork.save();

        return Ok();
    }

    // GET: Number of pending requests (for layout/sidebar)
    [HttpGet]
    public async Task<IActionResult> PendingCount()
    {
        int count = unitOfWork.CategoryRequestRepository.getAll().Count(r => r.Status == "pending" && !r.isDeleted);

        return Json(count);
    }
}
