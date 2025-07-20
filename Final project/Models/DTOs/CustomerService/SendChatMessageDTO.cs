using System.ComponentModel.DataAnnotations;

namespace Final_project.Models.DTOs.CustomerService
{
    public class SendChatMessageDTO
    {
        [Required]
        public string SessionId { get; set; }

        [Required]
        public string Message { get; set; }

        public string SenderId { get; set; }
    }
}
