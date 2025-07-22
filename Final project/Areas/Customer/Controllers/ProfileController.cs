using Final_project.Repository;
using Final_project.Services.Customer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Final_project.Areas.Customer.Controllers
{
    [Area("Customer")]
  
    public class ProfileController : Controller
    {
        private readonly UnitOfWork uof;

        public ProfileController(UnitOfWork uof)
        {
            this.uof = uof;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Orders(int page=1,int size=10)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = uof.OrderRepo.getAll().Where(o => o.buyer_id == userId).ToPagedResult(page,size);
               return View(orders);
        }
        public IActionResult orderDetails(string id)
        {
            var order = uof.OrderRepo.getById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
