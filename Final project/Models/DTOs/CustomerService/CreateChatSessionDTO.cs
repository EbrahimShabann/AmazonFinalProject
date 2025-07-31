using System.ComponentModel.DataAnnotations;

namespace Final_project.Models.DTOs.CustomerService
{
    public class CreateChatSessionDTO
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        public string SellerId { get; set; }
    }
}
