using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.Models
{
    public class wishlist
    {
        [Key]
        public string id { get; set; }

        [Required]
        public string user_id { get; set; }
        [ForeignKey("user_id")]
        public virtual ApplicationUser User { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        public virtual ICollection<wishlist_item> Items { get; set; } = new List<wishlist_item>();
    }
}
