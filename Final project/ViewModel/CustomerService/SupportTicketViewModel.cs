namespace Final_project.ViewModel.CustomerService
{
    public class SupportTicketViewModel
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int MessageCount { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
}
