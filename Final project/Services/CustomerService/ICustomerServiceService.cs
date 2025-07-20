using Final_project.Models;

namespace Final_project.Services.CustomerService
{
    public interface ICustomerServiceService
    {
        List<support_ticket> GetAllTickets();
        List<support_ticket> GetTicketsByUser(string userId);
        support_ticket GetTicketById(string ticketId);
        support_ticket CreateTicket(support_ticket ticket);
        void UpdateTicket(support_ticket ticket);
        void ResolveTicket(string ticketId, string resolvedBy);
        void DeleteTicket(string ticketId);
        List<support_ticket> GetTicketsByStatus(string status);
        List<support_ticket> GetTicketsByPriority(string priority);
        List<support_ticket> GetUnresolvedTickets();

        // Ticket Message operations
        List<ticket_message> GetTicketMessages(string ticketId);
        ticket_message SendTicketMessage(string ticketId, string senderId, string message);
        void MarkTicketMessagesAsRead(string ticketId, string userId);
        ticket_message GetLatestTicketMessage(string ticketId);

        // Ticket History operations
        List<ticket_history> GetTicketHistory(string ticketId);
        void LogTicketChange(string ticketId, string changedBy, string fieldChanged, string oldValue, string newValue);

        // Chat Session operations
        List<chat_session> GetUserChatSessions(string userId);
        chat_session GetChatSessionById(string sessionId);
        chat_session CreateOrGetChatSession(string customerId, string sellerId);
        void CloseChatSession(string sessionId);
        List<chat_session> GetActiveChatSessions();

        // Chat Message operations
        List<chat_message> GetChatMessages(string sessionId);
        chat_message SendChatMessage(string sessionId, string senderId, string message);
        void MarkChatMessagesAsRead(string sessionId, string userId);
        int GetUnreadChatMessageCount(string sessionId, string userId);
        chat_message GetLatestChatMessage(string sessionId);
    }
}
