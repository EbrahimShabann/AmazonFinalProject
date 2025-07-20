using Final_project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.ViewModel.Customer
{
    public class ProductsVM
    {
        public string id { get; set; }

        public string name { get; set; }
        public decimal? price { get; set; }
        public decimal? discount_price { get; set; }
        public int? stock_quantity { get; set; }
        public string Brand { get; set; }
        public string category_id { get; set; }
        [ForeignKey("category_id")]
        public virtual category Category { get; set; }

        public string seller_id { get; set; }
        [ForeignKey("seller_id")]
        public virtual ApplicationUser Seller { get; set; }

        public DateTime? created_at { get; set; } = DateTime.UtcNow;

        public string approved_by { get; set; }

        public string image_url { get; set; }
        public int? rating { get; set; }
    }
}
