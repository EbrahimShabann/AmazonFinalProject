namespace Final_project.ViewModel.CustomerService
{
    public class TicketDetailsViewModel
    {
        public SupportTicketViewModel Ticket { get; set; }
        public List<TicketMessageViewModel> Messages { get; set; }
        public List<TicketHistoryViewModel> History { get; set; }
    }
}
