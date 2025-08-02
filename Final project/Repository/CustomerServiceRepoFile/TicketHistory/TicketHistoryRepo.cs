using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CustomerServiceRepoFile.TicketHistory
{
    public class TicketHistoryRepo : ITicketHistoryRepo
    {
        private readonly AmazonDBContext _context;

        public TicketHistoryRepo(AmazonDBContext context)
        {
            _context = context;
        }
        public void add(ticket_history entity)
        {
            entity.id = Guid.NewGuid().ToString();
            _context.ticket_histories.Add(entity);
        }

        public List<ticket_history> getAll()
        {
            return _context.ticket_histories
                .Include(th => th.Support_Ticket)
                .OrderByDescending(th => th.changed_at)
                .ToList();
        }

        public ticket_history getById(string id)
        {
            return _context.ticket_histories
                .Include(th => th.Support_Ticket)
                .FirstOrDefault(th => th.id == id);
        }

        public void Update(ticket_history entity)
        {
            _context.ticket_histories.Update(entity);
        }

        public List<ticket_history> GetHistoryByTicketId(string ticketId)
        {
            return _context.ticket_histories
                .Include(th => th.Support_Ticket)
                .Where(th => th.ticket_id == ticketId)
                .OrderByDescending(th => th.changed_at)
                .ToList();
        }

        public void LogChange(string ticketId, string changedBy, string fieldChanged, string oldValue, string newValue)
        {
            var historyEntry = new ticket_history
            {
                id = Guid.NewGuid().ToString(),
                ticket_id = ticketId,
                changed_by = changedBy,
                changed_at = DateTime.UtcNow,
                field_changed = fieldChanged,
                old_value = oldValue,
                new_value = newValue
            };

            _context.ticket_histories.Add(historyEntry);
        }
    }
}
