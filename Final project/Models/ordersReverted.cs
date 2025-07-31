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
        public virtual order Order { get; set; }

        public string order_itemId { get; set; }
        [ForeignKey("order_itemId")]
        public virtual order_item Order_Item { get; set; }

        public DateTime RevertDate { get; set; }

        public string Reason { get; set; }
        public string Notes { get; set; }
    }
}
