using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.Models
{
    public class ordersReverted
    {
        [Key]
        public string id { get; set; } = Guid.NewGuid().ToString();

        public string orderId { get; set; }
        [ForeignKey("orderId")]
        [ValidateNever]
        public virtual order Order { get; set; }

        public string order_itemId { get; set; }
        [ForeignKey("order_itemId")]
        [ValidateNever]
        public virtual order_item Order_Item { get; set; }

        public DateTime RevertDate { get; set; }

        [Required]
        public string Reason { get; set; }
        public string Notes { get; set; }
        
    }
}
