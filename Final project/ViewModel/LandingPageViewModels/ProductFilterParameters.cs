namespace Final_project.ViewModel.LandingPageViewModels
{
    public class ProductFilterParameters
    {
        public string CategoryId { get; set; }
        public string SubcategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinRating { get; set; }
        public string SortBy { get; set; } = "relevance";
        public int PageSize { get; set; } = 20;
        public int Skip { get; set; } = 0;
    }
    public enum ProductSortOption
    {
        Relevance,
        PriceLowToHigh,
        PriceHighToLow,
        NewestArrivals,
        BestSelling,
        HighestRated
    }
}
