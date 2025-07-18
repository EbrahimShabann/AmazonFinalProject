using Final_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers.Seller
{
    public class AttributesConstraintsController : Controller
    {
        private readonly AmazonDBContext db;

        public AttributesConstraintsController(AmazonDBContext db)
        {
            this.db = db;
        }
        public IActionResult CheckProductNameUniq(string Name ,string id)
        {
            //check uniqness of product name 
            var product = db.products.FirstOrDefault(p => p.name == Name);
            if (product != null && id!=product.id)   // in case adding new one only or update another product with name that is existed already
            {
                return Json("This Product is already existed");
            }
            return Json(true);

        }
        //public IActionResult CheckCatNameUniq(string Name,int CategoryId)
        //{
        //    //check uniqness of category name 
        //    var cat = db.Categories.FirstOrDefault(w => w.Name == Name);
        //    if (cat != null && CategoryId == 0)
        //    {
        //        return Json("This Category is already existed");
        //    }
        //    return Json(true);

        //}
       
       
       
    }
}
