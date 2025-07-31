using Final_project.Models;

namespace Final_project.Repository.CustomerServiceRepoFile.TicketMessage
{
    public interface ITicketMessageRepo : IRepository<ticket_message>
    {
        List<ticket_message> GetMessagesByTicketId(string ticketId);
        List<ticket_message> GetUnreadMessages(string ticketId);
        void MarkMessagesAsRead(string ticketId, string userId);
        ticket_message GetLatestMessage(string ticketId);
    }
}
