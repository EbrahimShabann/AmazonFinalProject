using System.ComponentModel.DataAnnotations;

namespace Final_project.ViewModel
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string name { get; set; }

        public string description { get; set; }

        [StringLength(255, ErrorMessage = "Image name cannot exceed 255 characters")]
        public string? image_url { get; set; } // Store only the image name
        public string parent_category_id { get; set; }

    }
}
