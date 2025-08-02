using Final_project.Models;

namespace Final_project.Repository.CustomerServiceRepoFile.SupportTicket
{
    public interface ISupportTicketRepo : IRepository<support_ticket>
    {
        List<support_ticket> GetTicketsByUserId(string userId);
        List<support_ticket> GetTicketsByStatus(string status);
        List<support_ticket> GetTicketsByPriority(string priority);
        List<support_ticket> GetUnresolvedTickets();
        List<support_ticket> GetTicketsByDateRange(DateTime startDate, DateTime endDate);
        void SoftDelete(string ticketId);
        void ResolveTicket(string ticketId, string resolvedBy);
    }
}
