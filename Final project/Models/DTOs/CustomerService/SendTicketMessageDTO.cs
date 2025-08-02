using System.ComponentModel.DataAnnotations;

namespace Final_project.Models.DTOs.CustomerService
{
    public class SendTicketMessageDTO
    {
        [Required]
        public string TicketId { get; set; }

        [Required]
        public string Message { get; set; }

        public string SenderId { get; set; }
    }
}
