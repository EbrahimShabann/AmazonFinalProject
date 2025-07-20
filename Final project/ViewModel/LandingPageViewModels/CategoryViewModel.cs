namespace Final_project.ViewModel.LandingPageViewModels
{
    public class CategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ParentCategoryName { get; set; }
        public List<CategoryViewModel> ChildCategories { get; set; } = new List<CategoryViewModel>();
    }
}
