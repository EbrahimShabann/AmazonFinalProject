using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.Models
{
    public class wishlist_item
    {
        [Key]
        public string id { get; set; }

        [Required]
        public string wishlist_id { get; set; }
        [ForeignKey("wishlist_id")]
        public virtual wishlist Wishlist { get; set; }

        [Required]
        public string product_id { get; set; }
        [ForeignKey("product_id")]
        public virtual product Product { get; set; }

        public DateTime added_at { get; set; } = DateTime.UtcNow;
    }
}
