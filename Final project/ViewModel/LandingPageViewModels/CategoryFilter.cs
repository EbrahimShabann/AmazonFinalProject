using Microsoft.AspNetCore.Mvc;

namespace Final_project.ViewModel.LandingPageViewModels
{
    public class CategoryFilter
    {
        public string? categoryId { get; set; }
        public string? categoryName { get; set; }
        public string? filter { get; set; }
        public string? SearchTerm { get; set; }

        // Add any other properties you need for filtering
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinRating { get; set; }
        public string? SortBy { get; set; }

    }
}
