namespace Final_project.ViewModel.CustomerService
{
    public class ChatSessionViewModel
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string SellerName { get; set; }
        public string CustomerId { get; set; }
        public string SellerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public string Status { get; set; }
        public int UnreadCount { get; set; }
        public string LastMessage { get; set; }
    }
}
