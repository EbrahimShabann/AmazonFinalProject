using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Final_project.CustomAttribute;

namespace Final_project.Models
{
    public class CategoryRequest
    {
        [Key]
        public string requredId{ get; set; }
        public string SellerId { get; set; }
        [ForeignKey("SellerId")]
        public virtual ApplicationUser User { get; set; }
        [UniqueCategoryName]
        public string CategoryName { get; set; }
        public string?  CategoryDiscription  { get; set; }
        public string Status { get; set; } = "pending";
        public string? AdminComment  { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
