using System;

namespace Final_project.ViewModel.NewFolder
{
    public class ChatbotProductViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string ImageUrl { get; set; }
        public int Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Description { get; set; }
        public int TotalSold { get; set; }
        public bool Prime { get; set; }
        public DateTime DeliveryTiming { get; set; }
        public int StockQuantity { get; set; }
        public string Colors { get; set; }
        public string Sizes { get; set; }
        public string Sku { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string CategoryId { get; set; }
    }
}