namespace Final_project.ViewModel.CustomerService
{
    public class TicketMessageViewModel
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime? SentAt { get; set; }
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public bool? IsRead { get; set; }
        public bool IsFromUser { get; set; }
    }
}
