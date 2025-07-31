namespace Final_project.ViewModel.CustomerService
{
    public class ChatDetailsViewModel
    {
        public ChatSessionViewModel Session { get; set; }
        public List<ChatMessageViewModel> Messages { get; set; }
        public string CurrentUserId { get; set; }
    }
}
