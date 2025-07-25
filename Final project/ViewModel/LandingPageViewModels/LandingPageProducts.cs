namespace Final_project.ViewModel.NewFolder
{
    public class LandingPageProducts
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal? DiscountPercentage
        {
            get
            {
                if (Price == null || Price <= 0 || DiscountPrice == null)
                    return null;
                decimal discount = (Price.Value - DiscountPrice.Value) / Price.Value * 100;
                return Math.Round(discount, 1);
            }
        }
        public int ratting { get; set; }
        public int rattingStarMinuse { get; set; }
        public int ratingCount { get; set; }
        public DateTime delaviryTiming { get; set; }
        public bool prime { get; set; }
        public string Description { get; set; }


    }
}
