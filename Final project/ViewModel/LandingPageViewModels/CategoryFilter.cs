using Microsoft.AspNetCore.Mvc;

namespace Final_project.ViewModel.LandingPageViewModels
{
    public class CategoryFilter
    {
        public string categoryId { get; set; } = null;
        public string categoryName { get; set; } = null;
        public string filter { get; set; } = null;
    }
}
