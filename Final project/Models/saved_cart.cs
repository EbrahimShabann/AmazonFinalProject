using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.Models
{
    public class saved_cart
    {
        [Key]
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string user_id { get; set; }
        [ForeignKey("user_id")]
        public virtual ApplicationUser User { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? last_updated_at { get; set; } = DateTime.UtcNow;
        public virtual ICollection<saved_cart_item> SavedCartItems { get; set; }
    }
}