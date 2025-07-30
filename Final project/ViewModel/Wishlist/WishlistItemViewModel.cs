namespace Final_project.ViewModel.Wishlist
{
    public class WishlistItemViewModel
    {
        public string ItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
    }
}