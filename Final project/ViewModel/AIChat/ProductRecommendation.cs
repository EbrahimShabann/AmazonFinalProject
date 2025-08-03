namespace Final_project.ViewModel.AIChat
{
    public class ProductRecommendation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string ImageUrl { get; set; }
        public int Rating { get; set; }
        public int ReviewsCount { get; set; }
    }
}
