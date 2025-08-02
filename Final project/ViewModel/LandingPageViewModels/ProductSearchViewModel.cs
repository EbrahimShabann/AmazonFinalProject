namespace Final_project.ViewModel.LandingPageViewModels
{
    public class ProductSearchViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public double? DiscountPrice { get; set; }
        public string Brand { get; set; } = "Amazon";
        public int Rating { get; set; } = 4;
        public int ReviewCount { get; set; } = 100;
        public string Description { get; set; }
        public bool Prime { get; set; } = false;
    }
}
