namespace Final_project.ViewModel.Cart
{
    public class CartItemViewModel
    {
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public string Badge { get; set; }
    }
}
