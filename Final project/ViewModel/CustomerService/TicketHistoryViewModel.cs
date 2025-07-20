namespace Final_project.ViewModel.CustomerService
{
    public class TicketHistoryViewModel
    {
        public string Id { get; set; }
        public DateTime? ChangedAt { get; set; }
        public string FieldChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; }
    }
}
