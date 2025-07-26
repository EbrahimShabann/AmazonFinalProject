using System.ComponentModel.DataAnnotations;

namespace Final_project.Models.DTOs.CustomerService
{
    public class UpdateSupportTicketDTO
    {
        [Required]
        public string Id { get; set; }

        [StringLength(255)]
        public string Subject { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } // Open, In Progress, Resolved, Closed

        public string Priority { get; set; } // Low, Medium, High, Critical
    }
}
