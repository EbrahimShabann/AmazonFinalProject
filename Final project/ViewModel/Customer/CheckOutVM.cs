using Final_project.Models;

namespace Final_project.ViewModel.Customer
{
    public class CheckOutVM
    {
        public string shipping_address { get; set; }
        public string payment_method { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public decimal ShippingTax { get; set; } = 0;
        public List<CartVM> Carts { get; set; } = new List<CartVM>();

        // Helper property to calculate total
        public int TotalItems => Carts.Count;
        public decimal TotalPrice => Carts.Sum(c => (c.price ?? 0) * c.Quantity);
    }

    public class CartVM
    {
        public string ProductId { get; set; }
        public string seller_id { get; set; }

        public string imageUrl { get; set; }
        public string ProductName { get; set; }
        public decimal? price { get; set; }
        public decimal? originalPrice { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public string ProductColor { get; set; }
        public string ProductSize { get; set; }

        // Helper property to calculate item total
        public decimal ItemTotal => (price ?? 0) * Quantity;
    }
}