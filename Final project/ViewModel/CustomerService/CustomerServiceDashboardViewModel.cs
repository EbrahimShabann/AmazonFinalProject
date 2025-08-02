namespace Final_project.ViewModel.CustomerService
{
    public class CustomerServiceDashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int HighPriorityTickets { get; set; }
        public int ActiveChatSessions { get; set; }
        public List<SupportTicketViewModel> RecentTickets { get; set; }
        public List<ChatSessionViewModel> RecentChats { get; set; }
    }
}
