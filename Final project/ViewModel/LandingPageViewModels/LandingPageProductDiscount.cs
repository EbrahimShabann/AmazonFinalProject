using Final_project.Models;
using System.Collections.Generic;

namespace Final_project.ViewModel.NewFolder
{
    public class LandingPageProductDiscount
    {

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal? DiscountPercentage =>
            Price > 0 ? (Price - DiscountPrice) / Price * 100 : 0;
        public int ratting { get; set; }
        public int rattingStarMinuse { get; set; }
        public int ratingCount { get; set; }
        public DateTime delaviryTiming { get; set; }
        public bool prime { get; set; }


        //public class TopSellingProductVM
        //{
        //    public string ProductId { get; set; }
        //    public string ProductName { get; set; }
        //    public string Brand { get; set; }
        //    public string ImageUrl { get; set; }
        //    public decimal? Price { get; set; }
        //    public decimal? DiscountPrice { get; set; }
        //    public int TotalSold { get; set; }
        //    public decimal TotalRevenue { get; set; }
        //    public decimal? DiscountPercentage =>
        //        Price > 0 ? (Price - DiscountPrice) / Price * 100 : 0;
        //}

    }
}
