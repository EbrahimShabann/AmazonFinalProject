using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Final_project.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(255)]
        public string? profile_picture_url { get; set; }
        [StringLength(255)]
        public string? google_id { get; set; }
        public DateTime? date_of_birth { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime last_login { get; set; } = DateTime.UtcNow;
        public bool is_active { get; set; } = false;
        public bool is_deleted { get; set; } =false;
        public DateTime? deleted_at { get; set; }

        public virtual ICollection<chat_session> ChatSessionsAsCustomer { get; set; }
        public virtual ICollection<chat_session> ChatSessionsAsSeller { get; set; }
        public virtual ICollection<chat_message> ChatMessages { get; set; }
        public virtual ICollection<discount> Discounts { get; set; }
        public virtual ICollection<order> OrdersAsBuyer { get; set; }
        public virtual ICollection<order_item> OrdersAsSeller { get; set; }
        public virtual ICollection<product> Products { get; set; }
        public virtual ICollection<product_review> ProductReviews { get; set; }
        public virtual ICollection<shopping_cart> ShoppingCarts { get; set; }
        public virtual ICollection<support_ticket> SupportTickets { get; set; }
        public virtual ICollection<ticket_message> TicketMessages { get; set; }
    }
}
