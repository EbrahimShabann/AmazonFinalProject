using Final_project.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.ViewModel.Seller
{
    public class CreateProductVM
    {
        public string id { get; set; }

        [Remote("CheckProductNameUniq", "AttributesConstraints", AdditionalFields = "id", ErrorMessage = "This Product is already existed")]
        public string name { get; set; }
        public decimal? price { get; set; }
        public string description { get; set; }
        public List<string> SelectedColors { get; set; } = new List<string>();
        public  List<string> AvailableColors = new List<string>
{
    "Red",
    "Green",
    "Blue",
    "Yellow",
    "Orange",
    "Purple",
    "Pink",
    "Brown",
    "Black",
    "White",
    "Gray",
    "Silver",
    "Gold",
    "Beige",
    "Turquoise"
};
        public List<string> SelectedSizes { get; set; } = new List<string>();
        public  List<string> AvailableSizes = new List<string>
{
    "XS",
    "S",
    "M",
    "L",
    "XL",
    "XXL",
    "XXXL",
    "XXXXL"
};
        public decimal? discount_price { get; set; }
       
        public int? stock_quantity { get; set; }
        public string Brand { get; set; }
        public string category_id { get; set; }
        [ForeignKey("category_id")]
        public virtual category Category { get; set; }

        public List<product_image> ExistingImages { get; set; }

        public List<IFormFile> images { get; set; }=new List<IFormFile>();
    }
}
