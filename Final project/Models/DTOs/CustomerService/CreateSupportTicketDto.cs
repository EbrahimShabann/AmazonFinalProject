using System.ComponentModel.DataAnnotations;

namespace Final_project.Models.DTOs.CustomerService
{
    public class CreateSupportTicketDto
    {
        [Required]
        [StringLength(255)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Priority { get; set; } // Low, Medium, High, Critical

        public string UserId { get; set; }
    }
}
