using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.Models
{
    public class saved_cart_item
    {
        [Key]
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string saved_cart_id { get; set; }
        [ForeignKey("saved_cart_id")]
        public virtual saved_cart SavedCart { get; set; }
        public string product_id { get; set; }
        [ForeignKey("product_id")]
        public virtual product Product { get; set; }
        public int quantity { get; set; } = 1;
        public DateTime added_at { get; set; } = DateTime.UtcNow;
    }
}
