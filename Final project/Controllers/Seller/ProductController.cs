using Final_project.Models;
using Final_project.Repository;
using Final_project.Services.SellerPagination;
using Final_project.ViewModel.Seller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Final_project.Controllers.Seller
{
    //[Authorize(Roles ="seller")]
    public class ProductController : Controller
    {
        private readonly UnitOfWork uof;
        private readonly AmazonDBContext db;

        public ProductController(UnitOfWork uof, AmazonDBContext db)
        {
            this.uof = uof;
            this.db = db;
        }
        public IActionResult Index(int page = 1, int size = 10)
        {
            //var currentSellerId= User.Claims.FirstOrDefault(u=>u.Type==ClaimTypes.NameIdentifier)?.Value;

            var products = uof.ProductRepository.getProductsWithImages()   //.Where(p=>p.seller_id==currentSellerId)
                            .ToPagedResult(page, size);

            return View(products);
        }

        [HttpGet]
        public IActionResult UpSert(string id = null)
        {
            ViewBag.categories = db.categories.ToList();
            if (string.IsNullOrEmpty(id))
            {
                // Create new product
                return View(new CreateProductVM());
            }
            else
            {
                // Update existing product
                var product = uof.ProductRepository.getById(id);
                if (product == null)
                {
                    return NotFound();
                }
                var vm = new CreateProductVM
                {
                    id = product.id,
                    name = product.name,
                    price = product.price,
                    stock_quantity = product.stock_quantity,
                    Brand = product.Brand,
                    category_id = product.category_id,
                    //images = product.images.Select(i => i.image).ToList()
                };
                return View(vm);
            }

        }

        [HttpPost]
        public IActionResult UpSert(CreateProductVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.id == null)
                {
                    //create new product
                    var product = new product
                    {
                        id = Guid.NewGuid().ToString(),
                        name = vm.name,
                        Brand = vm.Brand,
                        price = vm.price,
                        stock_quantity = vm.stock_quantity,
                        category_id = vm.category_id,
                        created_at = DateTime.UtcNow,
                        is_active = true,
                        is_approved = false,
                        is_deleted = false,       
                        discount_price = vm.discount_price,
                        //seller_id = User.FindFirstValue(ClaimTypes.NameIdentifier) // Get the current seller's ID
                    };
                    foreach (var selectedSize in vm.SelectedSizes)
                    {
                        product.SelectedSizes.Add(selectedSize);
                    }
                    foreach (var selectedColor in vm.SelectedColors)
                    {
                        product.SelectedColors.Add(selectedColor);
                    }
                    uof.ProductRepository.add(product);
                    uof.save();
                    if (vm.images != null && vm.images.Count > 0)
                    {
                        // Create folder for each product
                        var productFolder = Path.Combine("wwwroot", "images", "products", $"product-{product.id}");

                        // Ensure directory exists
                        Directory.CreateDirectory(productFolder);
                        foreach (var img in vm.images)
                        {
                            //allowed extensions only
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var imgExtension = Path.GetExtension(img.FileName).ToLower();
                            if (!allowedExtensions.Contains(imgExtension))
                            {
                                ModelState.AddModelError("", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                                return View(vm);
                            }
                            //img size validation
                            if (img.Length>5*1024*1024) // 5 MB
                            {
                                ModelState.AddModelError("", "Image size should not exceed 5 MB.");
                                return View(vm);
                            }
                            if (img.Length > 0)
                            {
                                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);   
                                var fullPath= Path.Combine(productFolder, imgName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    img.CopyTo(stream);
                                }
                                var productImage = new product_image();
                                var firstImage = vm.images.First();
                                if (img == firstImage)
                                {
                                    productImage.is_primary = true;
                                }
                                else
                                {
                                    productImage.is_primary = false;
                                }

                                productImage.id = Guid.NewGuid().ToString();
                                productImage.image_url = $"products/product-{product.id}/{imgName}";
                                productImage.product_id = product.id;
                                productImage.uploaded_at = DateTime.Now;

                                uof.ProductImageRepo.add(productImage);

                            }
                           
                        }
                        uof.save();
                        return RedirectToAction("Index");
                    }
                }

            }
            return View(vm);
        }
   
    
    }
}
