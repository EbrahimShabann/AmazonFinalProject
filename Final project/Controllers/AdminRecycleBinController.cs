using Final_project.Models;
using Final_project.Repository;
using Final_project.ViewModel;
using Final_project.ViewModel.RecycleBinViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    [Authorize(Roles = "admin")]

    [Produces("application/json")]

    public class AdminRecycleBinController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminRecycleBinController(UnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager)
        {
            this.unitOfWork = unitOfWork;
            this._userManager = _userManager;

        }
        // GET: /AdminRecycleBin
        public async Task<IActionResult> Index()
        {
            var deletedCategories = unitOfWork.CategoryRepository.GetAll(c => c.is_deleted)
                .ToList();

            var deletedProducts = unitOfWork.ProductRepository.GetAll(p => p.is_deleted)
                .ToList();
            var DeletedSupport = (await _userManager.GetUsersInRoleAsync("customerService"));
            var DeletedCustomerService = DeletedSupport.Where(c => c.is_deleted).ToList();

            var DeletedSeller = (await _userManager.GetUsersInRoleAsync("Seller")).Where(c => c.is_deleted).ToList();
            var Users = unitOfWork.UserRepository.GetAll().ToList();
            RecycleBinViewModel recycleBin = new RecycleBinViewModel
            {
                DeletedCategories = deletedCategories.Select(c => new DeletedCategories
                {
                    id = c.id,
                    name = c.name,
                    ParentCategory = c.parent_category_id != null ? unitOfWork.CategoryRepository.GetByIdAsync(c.parent_category_id).Result?.name : "None",
                    DeletedByUser = c.deleted_by != null ? unitOfWork.UserRepository.GetByIdAsync(c.deleted_by).Result?.UserName : "System",
                    img_Url = c.image_url,
                    deletedByImg = c.deleted_by != null ? unitOfWork.UserRepository.GetByIdAsync(c.deleted_by).Result?.profile_picture_url : "https://via.placeholder.com/150"
                }).ToList(),
                DeletedProducts = deletedProducts.Select(p => new DeletedProducts
                {
                    id = p.id,
                    name = p.name,
                    categoryName = unitOfWork.CategoryRepository.GetByIdAsync(p.category_id).Result?.name ?? "Unknown",
                    sellerName = unitOfWork.UserRepository.GetByIdAsync(p.seller_id).Result?.UserName ?? "Unknown",
                    img_Url = p.product_images != null && p.product_images.Count > 0 ? p.product_images.FirstOrDefault().image_url : "https://via.placeholder.com/150",
                    sellerImg = unitOfWork.UserRepository.GetByIdAsync(p.seller_id).Result?.profile_picture_url ?? "https://via.placeholder.com/150"
                }).ToList(),
                DeletedCustomerService = DeletedCustomerService.Select(c => new DeletedUsers
                {
                    id = c.Id,
                    name = c.UserName,
                    deleted_at = c.deleted_at,
                    img_url = c.profile_picture_url
                }).ToList(),
                DeletedSellers = DeletedSeller.Select(c => new DeletedUsers
                {
                    id = c.Id,
                    name = c.UserName,
                    deleted_at = c.deleted_at,
                    img_url = c.profile_picture_url

                }).ToList(),
                Users = Users
            };
            foreach (var product in deletedProducts)
            {
                product.Seller = await unitOfWork.UserRepository.GetByIdAsync(product.seller_id);
                product.category = await unitOfWork.CategoryRepository.GetByIdAsync(product.category_id);
                product.product_images = unitOfWork.ProductImageRepository.GetAll(img => img.product_id == product.id).ToList();
            }
            foreach (var category in deletedCategories)
            {
                category.CreatedByUser = await unitOfWork.UserRepository.GetByIdAsync(category.created_by);
                category.ParentCategory = await unitOfWork.CategoryRepository.GetByIdAsync(category.parent_category_id);
                category.DeletedByUser = await unitOfWork.UserRepository.GetByIdAsync(category.deleted_by);
                category.LastModifiedByUser = await unitOfWork.UserRepository.GetByIdAsync(category.last_modified_by);
            }


            return View(recycleBin);
        }

        // POST: Restore Category
        [HttpPost]
        public async Task<IActionResult> RestoreCategory(string id)
        {

            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null || !category.is_deleted)
                return Json(new { success = false, message = "Category not found." });
            var products = unitOfWork.ProductRepository.GetAll(p => p.category_id == id);
            category.is_deleted = false;
            category.is_active = false;
            foreach (product p in products)
            {
                p.is_deleted = false;
                p.is_active = false;
                p.is_approved = true;
            }
            unitOfWork.save();

            return Json(new { success = true, message = "Category restored successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> HardDeleteCategory(string id)
        {
            try
            {
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null || !category.is_deleted)
                    return Json(new { success = false, message = "Category not found." });

                // Get all products in this category
                var products = unitOfWork.ProductRepository
                    .GetAll(p => p.category_id == id)
                    .ToList();

                // Delete all dependencies
                foreach (var product in products)
                {
                    await DeleteProductDependencies(product.id);
                    unitOfWork.ProductRepository.Delete(product);
                }

                unitOfWork.CategoryRepository.Delete(category);
                unitOfWork.save();

                return Json(new { success = true, message = "Category deleted permanently." });
            }
            catch (DbUpdateException ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Database error: {ex.InnerException?.Message ?? ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                }));
            }
        }
        // POST: Restore Product
        [HttpPost]
        public async Task<IActionResult> RestoreProduct(string id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null || !product.is_deleted)
                return Json(new { success = false, message = "Product not found." });

            product.is_deleted = false;
            product.is_active = false;
            product.is_approved = true;
            unitOfWork.save();

            return Json(new { success = true, message = "Product restored successfully." });
        }

        // POST: Permanently Delete Product
        [HttpPost]
        public async Task<IActionResult> HardDeleteProduct(string id)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
                if (product == null || !product.is_deleted)
                    return Json(new { success = false, message = "Product not found." });

                await DeleteProductDependencies(id);
                unitOfWork.ProductRepository.Delete(product);
                unitOfWork.save();

                return Json(new { success = true, message = "Product deleted permanently." });
            }
            catch (DbUpdateException ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Database error: {ex.InnerException?.Message ?? ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                }));
            }
        }

        // POST: Restore customer service
        [HttpPost]
        public async Task<IActionResult> RestoreCastomerService(string id)
        {
            var CastomerService = await unitOfWork.UserRepository.GetByIdAsync(id);
            if (CastomerService == null || !CastomerService.is_deleted)
                return Json(new { success = false, message = "Customer Service not found." });

            CastomerService.is_deleted = false;
            CastomerService.is_active = false;
            unitOfWork.save();

            return Json(new { success = true, message = "Customer Service restored successfully." });
        }

        // POST: Permanently Delete customer service
        [HttpPost]
        public async Task<IActionResult> HardDeleteCustomerService(string id)
        {
            var CastomerService = await unitOfWork.UserRepository.GetByIdAsync(id);
            if (CastomerService == null || !CastomerService.is_deleted)
                return Json(new { success = false, message = "Customer Service not found." });

            unitOfWork.UserRepository.Delete(CastomerService);
            unitOfWork.save();

            return Json(new { success = true, message = "Customer Service deleted permanently." });
        }

        // POST: Restore seller
        [HttpPost]
        public async Task<IActionResult> RestoreSeller(string id)
        {
            var seller = await unitOfWork.UserRepository.GetByIdAsync(id);
            if (seller == null || !seller.is_deleted)
                return Json(new { success = false, message = "Seller not found." });

            var products = unitOfWork.ProductRepository.GetAll(p => p.category_id == id);

            foreach (product p in products)
            {
                p.is_deleted = false;
                p.is_active = false;
                p.is_approved = true;
            }


            seller.is_deleted = false;
            seller.is_active = false;
            unitOfWork.save();

            return Json(new { success = true, message = "Seller restored successfully." });
        }

        // POST: Permanently Delete seller
        [HttpPost]
        public async Task<IActionResult> HardDeleteSeller(string id)
        {
            try
            {
                var seller = await unitOfWork.UserRepository.GetByIdAsync(id);
                if (seller == null || !seller.is_deleted)
                    return Json(new { success = false, message = "Seller not found." });

                var products = unitOfWork.ProductRepository
                    .GetAll(p => p.seller_id == id)
                    .ToList();

                foreach (var product in products)
                {
                    await DeleteProductDependencies(product.id);
                    unitOfWork.ProductRepository.Delete(product);
                }

                unitOfWork.UserRepository.Delete(seller);
                unitOfWork.save();

                return Json(new { success = true, message = "Seller deleted permanently." });
            }
            catch (DbUpdateException ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Database error: {ex.InnerException?.Message ?? ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                }));
            }
        }

        // Helper method to delete all product dependencies
        private async Task DeleteProductDependencies(string productId)
        {
            // Delete cart items
            var cartItems = unitOfWork.CartItemRepository
                .getAll().Where(c => c.product_id == productId)
                .ToList();

            foreach (var item in cartItems)
            {
                unitOfWork.CartItemRepository.Remove(item);
            }

            // Delete order items
            var orderItems = unitOfWork.OrderItemRepository
                .GetAll(o => o.product_id == productId)
                .ToList();

            foreach (var item in orderItems)
            {
                unitOfWork.OrderItemRepository.Delete(item);
            }
            var productImgs = unitOfWork.ProductImageRepository.GetAll(img => img.product_id == productId);
            foreach (var productImg in productImgs)
            {
                unitOfWork.ProductImageRepository.Delete(productImg);
            }
            var reviews = unitOfWork.ProductRepository.getProductReviews(productId);
            foreach (var review in reviews)
            {
                foreach (var reply in review.replies)
                {
                    unitOfWork.ProductRepository.DeleteReviewReply(reply);
                }
                unitOfWork.ProductRepository.DeleteReview(review);
            }
            // Add more dependencies as needed
            await Task.CompletedTask;
        }

    }
}
