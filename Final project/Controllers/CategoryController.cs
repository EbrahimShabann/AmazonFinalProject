using Final_project.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class CategoryController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public CategoryController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetCategorys()
        {
            return Json(unitOfWork.CategoryRepository.GetCategoryWithItsChildern());
        }
    }
}
