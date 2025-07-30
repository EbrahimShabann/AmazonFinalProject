using Final_project.Models;

namespace Final_project.Repository.CustomerServiceRepoFile.TicketHistory
{
    public interface ITicketHistoryRepo : IRepository<ticket_history>
    {
        List<ticket_history> GetHistoryByTicketId(string ticketId);
        void LogChange(string ticketId, string changedBy, string fieldChanged, string oldValue, string newValue);
    }
}
