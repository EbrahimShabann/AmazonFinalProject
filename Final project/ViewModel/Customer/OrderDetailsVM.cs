using Final_project.Models;

namespace Final_project.ViewModel.Customer
{
    public class OrderDetailsVM
    {
        public order Order { get; set; }
        public order_history OrderHistory { get; set; }
        public List<order_item> OrderItems { get; set; }
    }
}
