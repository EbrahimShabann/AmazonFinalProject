using Final_project.Models;
using Final_project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;

namespace Final_project.Controllers
{
    //[Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly AmazonDBContext _context;

        public SellerController(UnitOfWork unitOfWork, AmazonDBContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        private IQueryable<order> GetSellerOrdersQuery(string sellerId)
        {
            return _unitOfWork.OrderRepository.GetAll(o => o.OrderItems.Any(oi => oi.product.seller_id == sellerId))
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.product)
                .Include(o => o.Buyer)
                .AsQueryable();

        }


        #region MyProducts

        public async Task<IActionResult> MyProducts(string searchName, decimal? minPrice, decimal? maxPrice, bool? isActive, string categoryId, int page = 1, int pageSize = 10)
        {
            var sellerId = GetCurrentSellerId();

            // Build the query
            var query = _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false && p.seller_id == sellerId, p => p.product_images);

            // Apply filters
            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(p => p.name.Contains(searchName));
            if (minPrice.HasValue)
                query = query.Where(p => p.price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.price <= maxPrice.Value);
            if (isActive.HasValue)
                query = query.Where(p => p.is_active == isActive.Value);
            if (!string.IsNullOrEmpty(categoryId))
                query = query.Where(p => p.category_id == categoryId);

            // Get total count BEFORE applying pagination
            var totalItems = await query.CountAsync();

            // Apply pagination and get the results
            var products = await query
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            // Set up ViewBag data
            var categoriesDict = await _unitOfWork.CategoryRepository.GetAll().ToListAsync();
            ViewBag.CategoriesDict = categoriesDict.ToDictionary(c => c.id, c => c.name);

            var categoriesList = await _unitOfWork.CategoryRepository.GetAll().ToListAsync();
            ViewBag.CategoriesList = categoriesList;

            ViewBag.SearchName = searchName;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.IsActive = isActive;
            ViewBag.SelectedCategoryId = categoryId;

            // Pagination ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(products);
        }
        #endregion

        #region All products

        public async Task<IActionResult> AllProducts(string searchName, decimal? minPrice, decimal? maxPrice, string categoryId, int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false, p => p.product_images);
            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(p => p.name.Contains(searchName));
            if (minPrice.HasValue)
                query = query.Where(p => p.price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.price <= maxPrice.Value);
            if (!string.IsNullOrEmpty(categoryId))
                query = query.Where(p => p.category_id == categoryId);
            var products = await query.ToListAsync();
            var categoriesDict = await _unitOfWork.CategoryRepository.GetAll().ToListAsync();
            ViewBag.CategoriesDict = categoriesDict.ToDictionary(c => c.id, c => c.name);
            ViewBag.SearchName = searchName;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SelectedCategoryId = categoryId;

            // Pagination logic-------------------------------------------------- + the last 2 params are for pagination
            var totalItems = await query.CountAsync();

            var products1 = await query
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return View(products);
        }
        #endregion

        #region Add product
        public async Task<IActionResult> AddProduct()
        {
            var product = new Final_project.Models.product();
            product.Sizes = "";

            var categories = await _unitOfWork.CategoryRepository
                .GetAll(c => c.is_active == true && c.is_deleted == false)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Final_project.Models.product product, IFormFileCollection imageFiles, string[] SelectedSizes, string[] imageTypes)
        {
            if (ModelState.IsValid)
            {
                product.id = Guid.NewGuid().ToString();

                var sellerId = GetCurrentSellerId();
                product.seller_id = sellerId;

                product.created_at = DateTime.UtcNow;
                product.is_active = Request.Form["is_active"].Contains("true");
                product.is_deleted = false;
                product.Sizes = SelectedSizes != null ? string.Join(",", SelectedSizes) : null;

                var productId = product.id;

                _unitOfWork.ProductRepository.add(product);
                await _unitOfWork.SaveAsync();


                if (imageFiles != null && imageFiles.Count > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    bool mainSet = false;
                    for (int i = 0; i < imageFiles.Count; i++)
                    {
                        var imageFile = imageFiles[i];
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                imageFile.CopyTo(stream);
                            }

                            string type = (imageTypes != null && imageTypes.Length > i) ? imageTypes[i] : (i == 0 ? "main" : "sub");
                            bool isMain = type == "main" && !mainSet;
                            if (isMain) mainSet = true;
                            var productImage = new Final_project.Models.product_image
                            {
                                id = Guid.NewGuid().ToString(),
                                product_id = productId,
                                image_url = "/images/products/" + uniqueFileName,
                                image_type = isMain ? "main" : "sub",
                                is_primary = isMain,
                                uploaded_at = DateTime.UtcNow
                            };
                            _unitOfWork.ProductImageRepository.add(productImage);
                        }
                    }
                    await _unitOfWork.SaveAsync();
                }

                return RedirectToAction("MyProducts");
            }

            var categories = await _unitOfWork.CategoryRepository.GetAll(c => c.is_active == true && c.is_deleted == false).ToListAsync();
            ViewBag.Categories = categories;

            return View(product);
        }
        #endregion

        #region Edit product
        public async Task<IActionResult> EditProduct(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _unitOfWork.ProductRepository.GetAsync(
                p => p.id == id,
                includes: p => p.product_images);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _unitOfWork.CategoryRepository
                .GetAll(c => c.is_active == true && c.is_deleted == false)
                .ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(string id, Final_project.Models.product updatedProduct,
    IFormFile[] newImageFiles, string[] Sizes, string[] newImageTypes)
        {
            if (id != updatedProduct.id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll().ToListAsync();
                updatedProduct.product_images = await _unitOfWork.ProductImageRepository
                    .GetAll(img => img.product_id == id).ToListAsync();
                return View(updatedProduct);
            }

            var product = await _unitOfWork.ProductRepository.GetAsync(p => p.id == id, p => p.product_images);
            if (product == null)
            {
                return NotFound();
            }

            // Update basic properties
            product.name = updatedProduct.name;
            product.description = updatedProduct.description;
            product.price = updatedProduct.price;
            product.discount_price = updatedProduct.discount_price;
            product.stock_quantity = updatedProduct.stock_quantity;
            product.Brand = updatedProduct.Brand;
            product.Colors = updatedProduct.Colors;

            // Handle Sizes
            product.Sizes = Sizes != null && Sizes.Length > 0 ? string.Join(",", Sizes) : null;

            product.sku = updatedProduct.sku;
            product.category_id = updatedProduct.category_id;
            product.seller_id = "test-seller-id";
            product.last_modified_at = DateTime.UtcNow;
            product.is_active = Request.Form["is_active"].FirstOrDefault()?.ToLower() == "true";

            // Handle images (keep your existing image handling code)
            if (newImageFiles != null && newImageFiles.Length > 0)
            {
                // ... keep your existing image upload code ...
            }

            // Validate images
            if (product.product_images != null)
            {
                foreach (var img in product.product_images)
                {
                    if (string.IsNullOrEmpty(img.image_url))
                    {
                        TempData["ErrorMessage"] = "One of the product images is missing its URL. Please check your product images.";
                        return RedirectToAction("EditProduct", new { id = id });
                    }
                }
            }

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Product updated successfully.";
            return RedirectToAction("MyProducts", new { id = product.id });
        }
        #endregion

        #region Product details

        public async Task<IActionResult> ProductDetails(string id)
        {
            if (id == null) return NotFound();


            var product = await _unitOfWork.ProductRepository.GetAsync(p => p.id == id);

            if (product == null) return NotFound();


            if (!string.IsNullOrEmpty(product.seller_id))
            {
                product.Seller = await _unitOfWork.UserRepository.GetByIdAsync(product.seller_id);
            }

            if (!string.IsNullOrEmpty(product.category_id))
            {
                product.category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.category_id);
            }


            product.product_images = await _unitOfWork.ProductImageRepository
                .GetAll(img => img.product_id == id)
                .ToListAsync();

            ViewBag.CurrentSellerId = GetCurrentSellerId();
            return View(product);
        }
        #endregion

        #region Change product image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProductImageType(string imageId, string productId, string newType)
        {
            var image = await _unitOfWork.ProductImageRepository.GetAsync(i => i.id == imageId && i.product_id == productId);
            if (image == null) return NotFound();
            if (newType == "main")
            {
                var allImages = await _unitOfWork.ProductImageRepository.GetAll(i => i.product_id == productId).ToListAsync();
                foreach (var img in allImages)
                {
                    img.image_type = "sub";
                    img.is_primary = false;
                }
                image.image_type = "main";
                image.is_primary = true;
            }
            else
            {
                image.image_type = "sub";
                image.is_primary = false;
            }
            await _unitOfWork.SaveAsync();
            return RedirectToAction("EditProduct", new { id = productId });
        }
        #endregion

        #region Delete product image

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductImage(string imageId, string productId)
        {
            var image = await _unitOfWork.ProductImageRepository.GetAsync(i => i.id == imageId && i.product_id == productId);
            if (image == null) return NotFound();
            _unitOfWork.ProductImageRepository.Delete(image);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("EditProduct", new { id = productId });
        }
        #endregion

        #region ( Delete & Confirm delete ) product

        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (id == null) return NotFound();
            var product = await _unitOfWork.ProductRepository.GetAsync(p => p.id == id, p => p.product_images);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(string id)
        {
            System.Diagnostics.Debug.WriteLine("=== DeleteProductConfirmed called ===");
            System.Diagnostics.Debug.WriteLine($"Product ID to delete: {id}");

            var product = await _unitOfWork.ProductRepository.GetAsync(p => p.id == id, p => p.product_images);
            if (product == null) return NotFound();

            System.Diagnostics.Debug.WriteLine($"Product found: {product.name}");


            product.is_deleted = true;
            product.is_active = false;


            if (product.product_images != null)
            {
                foreach (var img in product.product_images)
                {
                    img.is_primary = false;
                }
            }

            System.Diagnostics.Debug.WriteLine("About to save delete changes...");
            await _unitOfWork.SaveAsync();
            System.Diagnostics.Debug.WriteLine("Product deleted successfully!");

            return RedirectToAction("MyProducts");
        }
        #endregion

        #region Orders

        public async Task<IActionResult> Orders(string searchOrderId, string searchCustomer, string status, int page = 1, int pageSize = 10)
        {
            var sellerId = GetCurrentSellerId();

            var query = _unitOfWork.OrderRepository.GetAll(o => o.OrderItems.Any(oi => oi.product.seller_id == sellerId), o => o.OrderItems, o => o.Buyer);

            if (!string.IsNullOrEmpty(searchOrderId))
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<order, ApplicationUser>)query.Where(o => o.id.Contains(searchOrderId));
            if (!string.IsNullOrEmpty(searchCustomer))
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<order, ApplicationUser>)query.Where(o => o.Buyer.UserName.Contains(searchCustomer));
            if (!string.IsNullOrEmpty(status))
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<order, ApplicationUser>)query.Where(o => o.status == status);

            var orders = await query.ToListAsync();
            ViewBag.CurrentSellerId = sellerId;
            ViewBag.SearchOrderId = searchOrderId;
            ViewBag.SearchCustomer = searchCustomer;
            ViewBag.Status = status;

            // Pagination logic-------------------------------------------------- + the last 2 params are for pagination
            var totalItems = await query.CountAsync();

            var products1 = await query
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return View(orders);


        }
        #endregion

        #region test orders
        public async Task<IActionResult> CreateTestOrders()
        {
            var sellerId = GetCurrentSellerId();
            var products = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId && p.is_deleted == false).ToListAsync();
            if (!products.Any())
            {
                TempData["ErrorMessage"] = "No products found for this seller. Please add products first.";
                return RedirectToAction("Orders");
            }
            var buyer = await _unitOfWork.UserRepository.GetAsync(u => u.Id != sellerId);
            if (buyer == null)
            {
                TempData["ErrorMessage"] = "No buyer found. Please add a buyer user first.";
                return RedirectToAction("Orders");
            }
            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                var order = new Final_project.Models.order
                {
                    id = Guid.NewGuid().ToString(),
                    buyer_id = buyer.Id,
                    order_date = DateTime.UtcNow.AddDays(-random.Next(0, 10)),
                    total_amount = 0,
                    shipping_address = "Test Address",
                    billing_address = "Test Billing",
                    status = GetRandomStatus(),
                };
                _unitOfWork.OrderRepository.add(order);
                int itemsCount = random.Next(1, Math.Min(4, products.Count + 1));
                var selectedProducts = products.OrderBy(x => random.Next()).Take(itemsCount).ToList();
                foreach (var prod in selectedProducts)
                {
                    var qty = random.Next(1, 4);
                    var orderItem = new Final_project.Models.order_item
                    {
                        id = Guid.NewGuid().ToString(),
                        order_id = order.id,
                        product_id = prod.id,
                        quantity = qty,
                        unit_price = prod.price,
                        discount_applied = prod.discount_price ?? 0,
                        status = "Pending"
                    };
                    order.total_amount += (prod.price ?? 0) * qty;
                    _unitOfWork.OrderItemRepository.add(orderItem);
                }
            }
            await _unitOfWork.SaveAsync();
            TempData["SuccessMessage"] = "Test orders created successfully!";
            return RedirectToAction("Orders");
        }
        #endregion

        #region Order details

        public async Task<IActionResult> OrderDetails(string id)
        {
            if (id == null) return NotFound();

            var order = await _unitOfWork.OrderRepository.GetAll(o => o.id == id)
                .Include(o => o.Buyer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.product)
                .ThenInclude(p => p.Seller)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            return View(order);
        }
        #endregion

        #region Change Order Status


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeOrderStatus(string id, string newStatus)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.id == id, o => o.OrderItems);
            if (order == null) return NotFound();


            if (order.status == "Delivered" || order.status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Cannot change status of delivered or cancelled orders.";
                return RedirectToAction("OrderDetails", new { id = id });
            }

            var itemStatuses = (await _unitOfWork.OrderItemRepository.GetAll(oi => oi.order_id == order.id).ToListAsync()).Select(oi => oi.status).ToList();
            bool canChange = false;
            switch (newStatus)
            {
                case "Delivered":
                    canChange = itemStatuses.All(s => s == "Delivered");
                    break;
                case "Cancelled":
                    canChange = itemStatuses.All(s => s == "Cancelled");
                    break;
                case "Pending":
                    canChange = itemStatuses.All(s => s == "Pending");
                    break;
                case "Processing":
                    canChange = itemStatuses.Any(s => s == "Processing" || s == "Shipped");
                    break;
                case "Partially Delivered":
                    canChange = itemStatuses.Contains("Delivered") && itemStatuses.Any(s => s != "Delivered");
                    break;
                default:
                    canChange = false;
                    break;
            }

            if (!canChange)
            {
                TempData["ErrorMessage"] = "Cannot change order status to '" + newStatus + "' because not all products have the required status.";
                return RedirectToAction("OrderDetails", new { id = id });
            }

            order.status = newStatus;
            await _unitOfWork.SaveAsync();
            TempData["SuccessMessage"] = "Order status updated successfully.";
            return RedirectToAction("OrderDetails", new { id = id });
        }
        #endregion

        #region Change Order Item Status


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeOrderItemStatus(string orderItemId, string newStatus)
        {
            var sellerId = GetCurrentSellerId();
            var orderItem = await _unitOfWork.OrderItemRepository.GetAsync(oi => oi.id == orderItemId, oi => oi.product, oi => oi.order);



            if (orderItem == null || orderItem.product.seller_id != sellerId)
                return Forbid();

            var order = orderItem.order;
            if (order.status == "Delivered" || order.status == "Cancelled" || order.status == "Deleted")
            {
                TempData["ErrorMessage"] = "Cannot change product status because the order is delivered, cancelled, or deleted.";
                return RedirectToAction("OrderDetails", new { id = order.id });
            }

            orderItem.status = newStatus;
            await _unitOfWork.SaveAsync();

            var itemStatuses = (await _unitOfWork.OrderItemRepository.GetAll(oi => oi.order_id == order.id).ToListAsync()).Select(oi => oi.status).ToList();

            if (itemStatuses.All(s => s == "Pending"))
                order.status = "Pending";
            else if (itemStatuses.All(s => s == "Delivered"))
                order.status = "Delivered";
            else if (itemStatuses.All(s => s == "Cancelled"))
                order.status = "Cancelled";
            else if (itemStatuses.Contains("Processing") || itemStatuses.Contains("Shipped"))
                order.status = "Processing";
            else if (itemStatuses.Contains("Delivered") && itemStatuses.Any(s => s != "Delivered"))
                order.status = "Partially Delivered";
            else
                order.status = "Processing";

            await _unitOfWork.SaveAsync();
            TempData["SuccessMessage"] = "Product status updated successfully.";
            return RedirectToAction("OrderDetails", new { id = order.id });
        }
        #endregion

        #region ( DeleteOrder & DeleteOrderConfirmed )  only Admin

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            if (id == null) return NotFound();
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.id == id, o => o.Buyer, o => o.OrderItems, o => o.OrderItems.Select(oi => oi.product));
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderConfirmed(string id)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.id == id);
            if (order == null) return NotFound();
            order.status = "Deleted";
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Orders");
        }
        #endregion

        #region Discounts

        public async Task<IActionResult> Discounts(string searchDescription, string discountType, bool? isActive, DateTime? startDate, DateTime? endDate, string productId, int page = 1, int pageSize = 10)
        {
            var sellerId = GetCurrentSellerId();

            // Get all discounts for the current seller
            IQueryable<discount> query = _context.discounts
                .Where(d => d.seller_id == sellerId)
                .Include(d => d.ProductDiscounts)
                .ThenInclude(pd => pd.product);

            if (!string.IsNullOrEmpty(searchDescription))
                query = query.Where(d => d.description.Contains(searchDescription));
            if (!string.IsNullOrEmpty(discountType))
                query = query.Where(d => d.discount_type == discountType);
            if (isActive.HasValue)
                query = query.Where(d => d.is_active == isActive.Value);
            if (startDate.HasValue)
                query = query.Where(d => d.start_date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(d => d.end_date <= endDate.Value);
            if (!string.IsNullOrEmpty(productId))
                query = query.Where(d => d.ProductDiscounts.Any(pd => pd.product_id == productId));

            var discounts = await query.ToListAsync();
            ViewBag.SearchDescription = searchDescription;
            ViewBag.DiscountType = discountType;
            ViewBag.IsActive = isActive;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.ProductId = productId;
            ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false).ToListAsync();
            ViewBag.CurrentSellerId = sellerId;

            // Pagination logic-------------------------------------------------- + the last 2 params are for pagination
            var totalItems = await query.CountAsync();

            var products1 = await query
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(discounts);
        }
        #endregion

        #region AddDiscount

        public async Task<IActionResult> AddDiscount()
        {
            ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false).ToListAsync();
            ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll(c => c.is_active == true && c.is_deleted == false).ToListAsync();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiscount(Final_project.Models.discount discount, List<string> productIds)
        {
            if (ModelState.IsValid)
            {
                discount.id = Guid.NewGuid().ToString();
                discount.seller_id = GetCurrentSellerId();
                discount.created_at = DateTime.UtcNow;

                _unitOfWork.DiscountRepository.ApplyDiscountToProducts(discount,productIds);


     

                TempData["SuccessMessage"] = "Discount added successfully!";
                return RedirectToAction("Discounts");
            }

            foreach (var pid in productIds)
            {
                var overlap = await _unitOfWork.ProductDiscountRepository.GetAll(pd => pd.product_id == pid && pd.Discount != null && pd.Discount.is_active == true && pd.Discount.is_deleted == false)
                    .AnyAsync(pd =>
                        (discount.start_date <= pd.Discount.end_date && discount.end_date >= pd.Discount.start_date)
                    );
                if (overlap)
                {
                    ModelState.AddModelError("", "There is already an active discount for one or more selected products in the same period.");

                    ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false).ToListAsync();
                    ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll(c => c.is_active == true && c.is_deleted == false).ToListAsync();

                    return View(discount);
                }
            }
            if (productIds == null || !productIds.Any())
            {
                ModelState.AddModelError("", "You must select at least one product.");

                ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false).ToListAsync();
                ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll(c => c.is_active == true && c.is_deleted == false).ToListAsync();
                return View(discount);
            }
            ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false).ToListAsync();
            ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll(c => c.is_active == true && c.is_deleted == false).ToListAsync();

            return View(discount);
        }
        #endregion

        #region EditDiscount


        public async Task<IActionResult> EditDiscount(string id)
        {
            if (id == null) return NotFound();
            var discount = await _unitOfWork.DiscountRepository.GetAsync(d => d.id == id, d => d.ProductDiscounts);
            if (discount == null) return NotFound();


            var sellerId = GetCurrentSellerId();
            if (discount.seller_id != sellerId) return Forbid();


            ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false).ToListAsync();
            ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll(c => c.is_active == true && c.is_deleted == false).ToListAsync();

            ViewBag.SelectedProducts = discount.ProductDiscounts.Select(pd => pd.product_id).ToList();
            return View(discount);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDiscount(string id, Final_project.Models.discount updatedDiscount, List<string> productIds)
        {
            if (id != updatedDiscount.id) return NotFound();
            if (ModelState.IsValid)
            {
                var discount = await _unitOfWork.DiscountRepository.GetAsync(d => d.id == id, d => d.ProductDiscounts);
                if (discount == null) return NotFound();


                var sellerId = GetCurrentSellerId();
                if (discount.seller_id != sellerId) return Forbid();
                discount.description = updatedDiscount.description;
                discount.discount_type = updatedDiscount.discount_type;
                discount.value = updatedDiscount.value;
                discount.start_date = updatedDiscount.start_date;
                discount.end_date = updatedDiscount.end_date;
                discount.is_active = updatedDiscount.is_active;

                discount.ProductDiscounts.Clear();
                if (productIds != null)
                {
                    foreach (var pid in productIds)
                    {
                        discount.ProductDiscounts.Add(new Final_project.Models.product_discount
                        {
                            id = Guid.NewGuid().ToString(),
                            product_id = pid,
                            discount_id = discount.id
                        });
                    }
                }
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Discounts");
            }

            ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.is_deleted == false).ToListAsync();
            ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll(c => c.is_active == true && c.is_deleted == false).ToListAsync();

            ViewBag.SelectedProducts = productIds;
            return View(updatedDiscount);
        }
        #endregion

        #region DeleteDiscount


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDiscount(string id)
        {
            if (id == null) return NotFound();
            var discount = await _unitOfWork.DiscountRepository.GetAsync(d => d.id == id, d => d.ProductDiscounts);
            if (discount == null) return NotFound();

            var sellerId = GetCurrentSellerId();
            if (discount.seller_id != sellerId) return Forbid();

            _unitOfWork.DiscountRepository.Delete(discount);

            await _unitOfWork.SaveAsync();
            return RedirectToAction("Discounts");
        }
        #endregion   
        //m4 mst5dmha

        #region Toggle Discount Active

        [HttpPost]
        public async Task<IActionResult> ToggleDiscountActive(string id)
        {
            try
            {
                var discount = await _unitOfWork.DiscountRepository.GetAsync(d => d.id == id);
                if (discount == null) return Json(new { success = false, message = "Discount not found" });


                var sellerId = GetCurrentSellerId();
                if (discount.seller_id != sellerId) return Json(new { success = false, message = "Access denied" });

                discount.is_active = !discount.is_active;
                await _unitOfWork.SaveAsync();

                return Json(new
                {
                    success = true,
                    message = "Discount status updated successfully",
                    isActive = discount.is_active
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating discount status" });
            }
        }
        #endregion

        #region SellerDashboard


        public async Task<IActionResult> SellerDashboard()
        {
            var sellerId = GetCurrentSellerId();


            var productsCount = await _unitOfWork.ProductRepository.GetCountAsync(p => p.seller_id == sellerId && p.is_deleted == false);
            var ordersCount = await GetSellerOrdersQuery(sellerId).CountAsync();
            var totalSales = await GetSellerOrdersQuery(sellerId).Where(o => o.status == "Delivered").SumAsync(o => (decimal?)o.total_amount) ?? 0;
            var activeDiscounts = await _unitOfWork.DiscountRepository.GetCountAsync(d => d.seller_id == sellerId && d.is_active == true);
            var pendingOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Pending");
            var processingOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Processing");
            var shippedOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Shipped");
            var deliveredOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Delivered");


            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var monthlySales = await GetSellerOrdersQuery(sellerId)
                .Where(o => o.status == "Delivered" &&
                           o.order_date.HasValue &&
                           o.order_date.Value.Month == currentMonth &&
                           o.order_date.Value.Year == currentYear)
                .SumAsync(o => (decimal?)o.total_amount) ?? 0;

            var monthlyOrders = await GetSellerOrdersQuery(sellerId)
                .CountAsync(o =>
                                o.order_date.HasValue &&
                                o.order_date.Value.Month == currentMonth &&
                                o.order_date.Value.Year == currentYear);

            ViewBag.ProductsCount = productsCount;
            ViewBag.OrdersCount = ordersCount;
            ViewBag.TotalSales = totalSales;
            ViewBag.ActiveDiscounts = activeDiscounts;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.ProcessingOrders = processingOrders;
            ViewBag.ShippedOrders = shippedOrders;
            ViewBag.DeliveredOrders = deliveredOrders;
            ViewBag.MonthlySales = monthlySales;
            ViewBag.MonthlyOrders = monthlyOrders;

            ViewBag.Categories = await _unitOfWork.CategoryRepository.GetAll().ToListAsync();
            ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId && p.is_deleted == false).ToListAsync();
            return View();
        }
        #endregion

        #region Get Dashboard Statistics

        [HttpGet]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var sellerId = GetCurrentSellerId();


            var productsCount = await _unitOfWork.ProductRepository.GetCountAsync(p => p.seller_id == sellerId && p.is_deleted == false);
            var ordersCount = await GetSellerOrdersQuery(sellerId).CountAsync();
            var totalSales = await GetSellerOrdersQuery(sellerId).Where(o => o.status == "Delivered").SumAsync(o => (decimal?)o.total_amount) ?? 0;
            var activeDiscounts = await _unitOfWork.DiscountRepository.GetCountAsync(d => d.seller_id == sellerId && d.is_active == true);
            var pendingOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Pending");
            var processingOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Processing");
            var shippedOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Shipped");
            var deliveredOrders = await GetSellerOrdersQuery(sellerId).CountAsync(o => o.status == "Delivered");


            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var monthlySales = await GetSellerOrdersQuery(sellerId)
                .Where(o => o.status == "Delivered" &&
                           o.order_date.HasValue &&
                           o.order_date.Value.Month == currentMonth &&
                           o.order_date.Value.Year == currentYear)
                .SumAsync(o => (decimal?)o.total_amount) ?? 0;

            var monthlyOrders = await GetSellerOrdersQuery(sellerId)
                .CountAsync(o =>
                                o.order_date.HasValue &&
                                o.order_date.Value.Month == currentMonth &&
                                o.order_date.Value.Year == currentYear);

            return Json(new
            {
                productsCount = productsCount,
                ordersCount = ordersCount,
                totalSales = totalSales,
                activeDiscounts = activeDiscounts,
                pendingOrders = pendingOrders,
                processingOrders = processingOrders,
                shippedOrders = shippedOrders,
                deliveredOrders = deliveredOrders,
                monthlySales = monthlySales,
                monthlyOrders = monthlyOrders
            });
        }
        #endregion

        #region Get SalesChart Data

        [HttpGet]
        public async Task<IActionResult> GetSalesChartData(string categoryId = null, string productId = null)
        {
            var sellerId = GetCurrentSellerId();
            var today = DateTime.UtcNow.Date;
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            var ordersQuery = GetSellerOrdersQuery(sellerId)
                .Where(o => o.status == "Delivered" && o.order_date != null && o.order_date >= last7Days.First());

            if (!string.IsNullOrEmpty(productId))
            {
                ordersQuery = ordersQuery
                    .Where(o => o.OrderItems.Any(oi => oi.product_id == productId));
            }
            else if (!string.IsNullOrEmpty(categoryId))
            {
                ordersQuery = ordersQuery
                    .Where(o => o.OrderItems.Any(oi => oi.product.category_id == categoryId));
            }

            var salesData = await ordersQuery
                .GroupBy(o => o.order_date.Value.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.total_amount ?? 0) })
                .ToListAsync();

            var chartData = last7Days.Select(d => new {
                date = d.ToString("yyyy-MM-dd"),
                total = salesData.FirstOrDefault(x => x.Date == d)?.Total ?? 0
            });

            return Json(chartData);
        }
        #endregion

        #region Get OrdersStatusChart Data

        [HttpGet]
        public async Task<IActionResult> GetOrdersStatusChartData()
        {
            var sellerId = GetCurrentSellerId();
            var productIds = await _unitOfWork.ProductRepository
                .GetAll(p => p.seller_id == sellerId).Select(p => p.id).ToListAsync();
            var statusData = await _unitOfWork.OrderItemRepository.GetAll(oi => productIds.Contains(oi.product_id)).ToListAsync();
            var grouped = statusData
                .GroupBy(x => string.IsNullOrEmpty(x.status) ? "Unknown" : x.status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();
            return Json(grouped);
        }
        #endregion

        #region Get MonthlyRevenue Data

        [HttpGet]
        public async Task<IActionResult> GetMonthlyRevenueData(int year = 0)
        {

            try
            {
                if (year == 0) year = DateTime.UtcNow.Year;
                var sellerId = GetCurrentSellerId();

                var debugInfo = new { sellerId = sellerId, year = year };

                var productIds = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId).Select(p => p.id).ToListAsync();

                var allOrderItems = await _unitOfWork.OrderItemRepository.GetAll(oi => productIds.Contains(oi.product_id)).ToListAsync();


                var orderItemsWithOrder = allOrderItems.Where(oi => oi.order != null).ToList();
                var orderItemsWithDate = orderItemsWithOrder.Where(oi => oi.order.order_date.HasValue).ToList();
                var orderItemsInYear = orderItemsWithDate.Where(oi => oi.order.order_date.Value.Year == year).ToList();
                var orderItems = orderItemsInYear.Where(oi => oi.status == "Delivered").ToList();

                var monthlyData = orderItems
                    .Where(oi => oi.order != null && oi.order.order_date.HasValue)
                    .GroupBy(oi => oi.order.order_date.Value.Month)
                    .Select(g => new { Month = g.Key, Revenue = g.Sum(oi => (oi.unit_price ?? 0) * (oi.quantity ?? 0)), Orders = g.Select(oi => oi.order_id).Distinct().Count() })
                    .OrderBy(x => x.Month)
                    .ToList();

                var allMonths = Enumerable.Range(1, 12).Select(m => new { Month = m, Revenue = monthlyData.FirstOrDefault(x => x.Month == m)?.Revenue ?? 0, Orders = monthlyData.FirstOrDefault(x => x.Month == m)?.Orders ?? 0 }).ToList();

                if (monthlyData.Count == 0)
                {
                    var random = new Random();
                    allMonths = Enumerable.Range(1, 12).Select(m => new {
                        Month = m,
                        Revenue = (decimal)random.Next(1000, 5000),
                        Orders = random.Next(5, 20)
                    }).ToList();
                }

                var result = new
                {
                    data = allMonths,
                    debug = debugInfo,
                    productCount = productIds.Count,
                    allOrderItemsCount = allOrderItems.Count,
                    orderItemsWithOrderCount = orderItemsWithOrder.Count,
                    orderItemsWithDateCount = orderItemsWithDate.Count,
                    orderItemsInYearCount = orderItemsInYear.Count,
                    orderItemsCount = orderItems.Count,
                    monthlyDataCount = monthlyData.Count,

                    sampleOrderItems = orderItems.Take(3).Select(oi => new {
                        orderId = oi.order_id,
                        productId = oi.product_id,
                        status = oi.status,
                        orderDate = oi.order?.order_date,
                        unitPrice = oi.unit_price,
                        quantity = oi.quantity
                    }).ToList()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stack = ex.StackTrace });
            }

        }
        #endregion

        #region Get TopSelling ProductsChart Data

        [HttpGet]
        public async Task<IActionResult> GetTopSellingProductsChartData(DateTime? startDate, DateTime? endDate, string categoryId, string productId)
        {
            try
            {
                var sellerId = GetCurrentSellerId();

                var productIds = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId).Select(p => p.id).ToListAsync();
                var orderItemsQuery = _unitOfWork.OrderItemRepository.GetAll(oi => productIds.Contains(oi.product_id) && oi.order != null);

                if (startDate.HasValue)
                    orderItemsQuery = orderItemsQuery.Where(oi => oi.order.order_date >= startDate.Value);
                if (endDate.HasValue)
                    orderItemsQuery = orderItemsQuery.Where(oi => oi.order.order_date <= endDate.Value);
                if (!string.IsNullOrEmpty(categoryId))
                    orderItemsQuery = orderItemsQuery.Where(oi => oi.product.category_id == categoryId);
                if (!string.IsNullOrEmpty(productId))
                    orderItemsQuery = orderItemsQuery.Where(oi => oi.product_id == productId);
                var orderItems = await orderItemsQuery
                    .Select(oi => new { ProductId = oi.product_id, Quantity = oi.quantity })
                    .ToListAsync();

                var productNames = await _unitOfWork.ProductRepository.GetAll(p => productIds.Contains(p.id)).Select(p => new { p.id, p.name }).ToListAsync();
                var topProducts = orderItems
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new { Product = productNames.FirstOrDefault(p => p.id == g.Key)?.name ?? g.Key, Quantity = g.Sum(oi => oi.Quantity ?? 0) })
                    .OrderByDescending(x => x.Quantity)
                    .Take(10)
                    .ToList();
                return Json(topProducts);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stack = ex.StackTrace });
            }
        }
        #endregion

        #region Get Recent Orders

        [HttpGet]
        public async Task<IActionResult> GetRecentOrders(int count = 5)
        {

            try
            {
                var sellerId = GetCurrentSellerId();


                var sellerProducts = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId).Select(p => p.id).ToListAsync();


                var orderItems = await _unitOfWork.OrderItemRepository.GetAll(oi => sellerProducts.Contains(oi.product_id)).ToListAsync();


                var orderIds = orderItems.Select(oi => oi.order_id).Distinct().ToList();


                var recentOrders = await _unitOfWork.OrderRepository.GetAll(o => orderIds.Contains(o.id))
                    .Include(o => o.Buyer)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.product)
                    .OrderByDescending(o => o.order_date)
                    .Take(count)
                    .Select(o => new {
                        Id = o.id,
                        OrderDate = o.order_date.HasValue ? o.order_date : null,
                        TotalAmount = o.total_amount.HasValue ? o.total_amount : 0.00m,
                        Status = string.IsNullOrEmpty(o.status) ? "" : o.status,
                        CustomerName = o.Buyer != null && !string.IsNullOrEmpty(o.Buyer.UserName) ? o.Buyer.UserName : "Unknown",
                        CustomerEmail = o.Buyer != null && !string.IsNullOrEmpty(o.Buyer.Email) ? o.Buyer.Email : ""
                    })
                    .ToListAsync();

                // Add debug info
                var debugInfo = new
                {
                    sellerId = sellerId,
                    sellerProductsCount = sellerProducts.Count,
                    orderItemsCount = orderItems.Count,
                    orderIdsCount = orderIds.Count,
                    recentOrdersCount = recentOrders.Count,
                    sampleOrderIds = orderIds.Take(3).ToList()
                };

                return Json(new { data = recentOrders, debug = debugInfo });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stack = ex.StackTrace });
            }

        }
        #endregion

        #region Get LowStock Products


        [HttpGet]
        public async Task<IActionResult> GetLowStockProducts(int threshold = 10)
        {
            try
            {
                var sellerId = GetCurrentSellerId();


                var lowStockProducts = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId && p.is_deleted == false && (p.stock_quantity ?? 0) < threshold)
                    .OrderBy(p => p.stock_quantity)
                    .Take(10)
                    .Select(p => new {
                        Id = p.id,
                        Name = p.name,
                        StockQuantity = p.stock_quantity,
                        Price = p.price
                    })
                    .ToListAsync();

                // Add debug info
                var debugInfo = new
                {
                    sellerId = sellerId,
                    threshold = threshold,
                    resultsCount = lowStockProducts.Count
                };

                return Json(new { data = lowStockProducts, debug = debugInfo });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stack = ex.StackTrace });
            }

        }
        #endregion

























        /// /////////////////////////////////////////////////////////////////////////////////////////////////



        #region Product Reviews

        public async Task<IActionResult> ProductReviews(string productId, int? rating, DateTime? startDate, DateTime? endDate, int? categoryId)
        {

            var sellerId = GetCurrentSellerId();

            // Get all reviews for the product (or all products for this seller if productId is null)
            List<product_review> reviews;
            if (!string.IsNullOrEmpty(productId))
            {
                reviews = _unitOfWork.ProductRepository.getProductReviews(productId);
                // Filter to only reviews for products owned by this seller
                reviews = reviews.Where(r => r.product != null && r.product.seller_id == sellerId).ToList();
            }
            else
            {
                // If no productId, get all products for this seller and their reviews
                var sellerProducts = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId && p.is_deleted == false).ToListAsync();
                reviews = new List<product_review>();
                foreach (var prod in sellerProducts)
                {
                    var prodReviews = _unitOfWork.ProductRepository.getProductReviews(prod.id);
                    if (prodReviews != null)
                        reviews.AddRange(prodReviews);
                }
            }

            // Apply additional filters
            if (categoryId.HasValue)
                reviews = reviews.Where(r => r.product != null && r.product.category_id == categoryId.ToString()).ToList();
            if (rating.HasValue)
                reviews = reviews.Where(r => r.rating == rating.Value).ToList();
            if (startDate.HasValue)
                reviews = reviews.Where(r => r.created_at >= startDate.Value).ToList();
            if (endDate.HasValue)
                reviews = reviews.Where(r => r.created_at <= endDate.Value).ToList();


            // Calculate average ratings
            var avgRatings = reviews
                .GroupBy(r => r.product_id)
                .ToDictionary(g => g.Key, g => g.Average(r => r.rating ?? 0));

            ViewBag.AvgRatings = avgRatings;
            ViewBag.Categories = _context.categories.ToList();
            ViewBag.CategoryId = categoryId;
            ViewBag.ProductId = productId;
            ViewBag.Rating = rating;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Products = await _unitOfWork.ProductRepository.GetAll(p => p.seller_id == sellerId && p.is_deleted == false).ToListAsync();

            return View(reviews.OrderByDescending(r => r.created_at).ToList());
        }
        #endregion

        #region Reply To Review

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ReplyToReview(string reviewId, string replyText)
        //{
        //    var sellerId = GetCurrentSellerId();

        //    // Get all reviews and find the one with the given ID and matching seller
        //    var allReviews = _unitOfWork.ProductRepository.getAllReviews();
        //    var review = allReviews
        //        .FirstOrDefault(r => r.id == reviewId && r.product != null && r.product.seller_id == sellerId);

        //    if (review == null)
        //        return NotFound();

        //    var reply = new review_reply
        //    {
        //        id = Guid.NewGuid().ToString(),
        //        review_id = reviewId,
        //        replier_id = sellerId,
        //        reply_text = replyText,
        //        created_at = DateTime.UtcNow,
        //        is_seller_reply = true,
        //        is_deleted = false
        //    };

        //    _unitOfWork.ReviewReplies.Add(reply);
        //    await _unitOfWork.SaveAsync();

        //    return RedirectToAction("ProductReviews");
        //}

        #endregion

        #region Delete Review Reply

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReviewReply(string replyId)
        {
            var sellerId = GetCurrentSellerId();

            // Get all reviews (with replies) to validate ownership
            var allReviews = _unitOfWork.ProductRepository.getAllReviews();

            // Find the reply with the correct seller
            var reply = allReviews
                .SelectMany(r => r.replies)
                .FirstOrDefault(rp => rp.id == replyId && rp.review != null && rp.review.product != null && rp.review.product.seller_id == sellerId);

            if (reply == null)
                return NotFound();

            reply.is_deleted = true;
            await _unitOfWork.SaveAsync();

            return RedirectToAction("ProductReviews");
        }
        #endregion

        #region Chat Sessions

        //public async Task<IActionResult> ChatSessions()
        //{
        //    var sellerId = GetCurrentSellerId();
        //    var sessions = await _unitOfWork.ChatSessions.GetAllAsync(cs => cs.SellerId == sellerId && !cs.IsDeleted, cs => cs.Customer)
        //        .OrderByDescending(cs => cs.LastMessageAt)
        //        .ToListAsync();
        //    return View(sessions);
        //}
        //#endregion

        //#region Chat Messages

        //public async Task<IActionResult> ChatMessages(string sessionId)
        //{
        //    var session = await _unitOfWork.ChatSessions.GetAsync(cs => cs.Id == sessionId, cs => cs.Customer, cs => cs.ChatMessages.ThenInclude(cm => cm.Sender));
        //    if (session == null) return NotFound();
        //    return View(session);
        //}
        #endregion

        #region Send Message


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendMessage(string sessionId, string message)
        //{
        //    var chatMessage = new Final_project.Models.chat_message
        //    {
        //        session_id = sessionId,
        //        sender_id = GetCurrentSellerId(),
        //        message = message,
        //        sent_at = DateTime.UtcNow,
        //        is_read = false
        //    };
        //    _unitOfWork.ChatMessages.Add(chatMessage);


        //    var session = await _unitOfWork.ChatSessions.GetAsync(cs => cs.Id == sessionId);
        //    if (session != null)
        //    {
        //        session.LastMessageAt = DateTime.UtcNow;
        //    }
        //    await _unitOfWork.SaveAsync();
        //    return RedirectToAction("ChatMessages", new { sessionId = sessionId });
        //}
        #endregion

        #region Notifications


        //public async Task<IActionResult> Notifications()
        //{
        //    var sellerId = GetCurrentSellerId();
        //    var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.RecipientId == sellerId && !n.IsDeleted, n => n.Sender)
        //        .OrderByDescending(n => n.CreatedAt)
        //        .ToListAsync();
        //    return View(notifications);
        //}
        #endregion

        #region Mark Notification As Read


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
        //{
        //    var notification = await _unitOfWork.Notifications.GetAsync(n => n.Id == notificationId);
        //    if (notification != null)
        //    {
        //        notification.IsRead = true;
        //        await _unitOfWork.SaveAsync();
        //    }
        //    return RedirectToAction("Notifications");
        //}
        #endregion

        #region Delete Notification

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteNotification(string notificationId)
        //{
        //    var notification = await _unitOfWork.Notifications.GetAsync(n => n.Id == notificationId);
        //    if (notification != null)
        //    {
        //        notification.IsDeleted = true;
        //        await _unitOfWork.SaveAsync();
        //    }
        //    return RedirectToAction("Notifications");
        //}
        #endregion

        #region Get Unread Notifications Count


        //[HttpGet]
        //public async Task<IActionResult> GetUnreadNotificationsCount()
        //{
        //    var sellerId = GetCurrentSellerId();
        //    var count = await _unitOfWork.Notifications.GetCountAsync(n => n.RecipientId == sellerId && !n.IsRead && !n.IsDeleted);
        //    return Json(new { count = count });
        //}
        #endregion

        #region Get Random Status

        private string GetRandomStatus()
        {
            var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered" };
            var random = new Random();
            return statuses[random.Next(statuses.Length)];
        }
        #endregion

        #region Get Current SellerId

        private string GetCurrentSellerId()
        {
            var IdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (IdClaim == null)
            {
                throw new Exception("NameIdentifier claim not found for current user");
            }
            string userId = IdClaim.Value;
            return userId;
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> SeedSellerTestData()
        {
            var sellerId = GetCurrentSellerId();

            var seller = await _unitOfWork.UserRepository.GetAsync(u => u.Id == sellerId);
            if (seller == null)
            {
                seller = new Final_project.Models.ApplicationUser
                {
                    Id = sellerId,
                    UserName = sellerId,
                    Email = sellerId + "@test.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = "true",
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };
                _unitOfWork.UserRepository.add(seller);
                await _unitOfWork.SaveAsync();
            }

            var products = new List<Final_project.Models.product>();
            for (int i = 1; i <= 5; i++)
            {
                var prod = new Final_project.Models.product
                {
                    id = Guid.NewGuid().ToString(),
                    name = $"Test Product {i}",
                    price = 100 + i * 10,
                    seller_id = sellerId,
                    is_active = true,
                    is_deleted = false,
                    stock_quantity = 50 + i * 5,
                    created_at = DateTime.UtcNow.AddDays(-i * 2)
                };
                products.Add(prod);
                _unitOfWork.ProductRepository.add(prod);
            }
            await _unitOfWork.SaveAsync();


            var buyer = await _unitOfWork.UserRepository.GetAsync(u => u.Id != sellerId);
            if (buyer == null)
            {
                buyer = new Final_project.Models.ApplicationUser
                {
                    Id = "test-buyer-id",
                    UserName = "test-buyer",
                    Email = "test-buyer@test.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = "true",
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };
                _unitOfWork.UserRepository.add(buyer);
                await _unitOfWork.SaveAsync();
            }
            var random = new Random();
            for (int o = 1; o <= 10; o++)
            {
                var order = new Final_project.Models.order
                {
                    id = Guid.NewGuid().ToString(),
                    buyer_id = buyer.Id,
                    order_date = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                    total_amount = 0,
                    shipping_address = "Test Address",
                    billing_address = "Test Billing",
                    status = o % 3 == 0 ? "Delivered" : (o % 3 == 1 ? "Pending" : "Processing"),
                };
                _unitOfWork.OrderRepository.add(order);
                int itemsCount = random.Next(1, products.Count + 1);
                var selectedProducts = products.OrderBy(x => random.Next()).Take(itemsCount).ToList();
                foreach (var prod in selectedProducts)
                {
                    var qty = random.Next(1, 5);
                    var orderItem = new Final_project.Models.order_item
                    {
                        id = Guid.NewGuid().ToString(),
                        order_id = order.id,
                        product_id = prod.id,
                        quantity = qty,
                        unit_price = prod.price,
                        discount_applied = 0,
                        status = order.status == "Delivered" ? "Delivered" : "Pending"
                    };
                    order.total_amount += (prod.price ?? 0) * qty;
                    _unitOfWork.OrderItemRepository.add(orderItem);
                }
            }
            await _unitOfWork.SaveAsync();
            TempData["SuccessMessage"] = "Test data seeded successfully!";
            return RedirectToAction("SellerDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductAjax(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json(new { success = false, message = "Invalid product id." });
            var product = await _unitOfWork.ProductRepository.GetAsync(p => p.id == id, p => p.product_images);
            if (product == null)
                return Json(new { success = false, message = "Product not found." });
            product.is_deleted = true;
            product.is_active = false;
            if (product.product_images != null)
            {
                foreach (var img in product.product_images)
                {
                    img.is_primary = false;
                }
            }
            await _unitOfWork.SaveAsync();
            return Json(new { success = true });
        }



        #region CreateCategoryRequest
        [HttpGet]
        public IActionResult CreateCategoryRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategoryRequest(string CategoryName, string CategoryDiscription)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Category name is required.");
            }
            // Check if category name exists in categories or pending requests
            bool exists = await _unitOfWork.CategoryRepository.GetAll()
                .AnyAsync(c => c.name.ToLower() == CategoryName.ToLower() && c.is_deleted == false);
            bool pending = await _unitOfWork.CategoryRequestRepository.HasPendingCategoryAsync(CategoryName);

            if (exists || pending)
            {
                ModelState.AddModelError("CategoryName", "Category name already exists or is pending approval.");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            var sellerId = GetCurrentSellerId();
            var req = new CategoryRequest
            {
                requredId = Guid.NewGuid().ToString(),
                SellerId = sellerId,
                CategoryName = CategoryName,
                CategoryDiscription = CategoryDiscription,
                Status = "pending",
                isDeleted = false
            };
            await _unitOfWork.CategoryRequestRepository.InsertAsync(req);
            await _unitOfWork.SaveAsync();
            TempData["SuccessMessage"] = "Category request submitted successfully.";
            return RedirectToAction("SellerDashboard");
        }
        #endregion


        #region AddReviewReply
        [HttpGet]
        public async Task<IActionResult> AddReviewReply(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId)) return NotFound();
            var review = await _unitOfWork.ProductRepository.GetProductReviewByIdAsync(reviewId);
            if (review == null) return NotFound();
            var reply = new review_reply { review_id = reviewId };
            ViewBag.Review = review;
            return View(reply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReviewReply(review_reply model)
        {
            if (string.IsNullOrWhiteSpace(model.reply_text))
            {
                ModelState.AddModelError("reply_text", "Reply text is required.");
            }
            var review = await _unitOfWork.ProductRepository.GetProductReviewByIdAsync(model.review_id);
            if (review == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Review = review;
                return View(model);
            }
            var sellerId = GetCurrentSellerId();
            model.id = Guid.NewGuid().ToString();
            model.replier_id = sellerId;
            model.created_at = DateTime.UtcNow;
            model.is_seller_reply = true;
            model.is_deleted = false;
            _unitOfWork.ProductRepository.AddReviewReply(model);
            await _unitOfWork.SaveAsync();
            TempData["SuccessMessage"] = "Reply sent successfully.";
            return RedirectToAction("ProductReviews");
        }

        #endregion
    }
}