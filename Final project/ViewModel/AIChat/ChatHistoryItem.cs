namespace Final_project.ViewModel.AIChat
{
    public class ChatHistoryItem
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public List<ProductRecommendation> Products { get; set; }
    }
}


