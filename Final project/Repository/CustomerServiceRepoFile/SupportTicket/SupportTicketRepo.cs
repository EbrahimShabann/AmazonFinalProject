using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CustomerServiceRepoFile.SupportTicket
{
    public class SupportTicketRepo:ISupportTicketRepo
    {
        private readonly AmazonDBContext _context;

        public SupportTicketRepo(AmazonDBContext context)
        {
            _context = context;
        }

        public void add(support_ticket entity)
        {
            entity.id = Guid.NewGuid().ToString();
            _context.support_tickets.Add(entity);
        }

        public List<support_ticket> getAll()
        {
            return _context.support_tickets
                .Include(st => st.User)
                .Where(st => !st.is_deleted)
                .OrderByDescending(st => st.created_at)
                .ToList();
        }

        public support_ticket getById(string id)
        {
            return _context.support_tickets
                .Include(st => st.User)
                .FirstOrDefault(st => st.id == id && !st.is_deleted);
        }

        public void Update(support_ticket entity)
        {
            _context.support_tickets.Update(entity);
        }

        public List<support_ticket> GetTicketsByUserId(string userId)
        {
            return _context.support_tickets
                .Include(st => st.User)
                .Where(st => st.user_id == userId && !st.is_deleted)
                .OrderByDescending(st => st.created_at)
                .ToList();
        }

        public List<support_ticket> GetTicketsByStatus(string status)
        {
            return _context.support_tickets
                .Include(st => st.User)
                .Where(st => st.status == status && !st.is_deleted)
                .OrderByDescending(st => st.created_at)
                .ToList();
        }

        public List<support_ticket> GetTicketsByPriority(string priority)
        {
            return _context.support_tickets
                .Include(st => st.User)
                .Where(st => st.priority == priority && !st.is_deleted)
                .OrderByDescending(st => st.created_at)
                .ToList();
        }

        public List<support_ticket> GetUnresolvedTickets()
        {
            return _context.support_tickets
                .Include(st => st.User)
                .Where(st => st.resolved_at == null && !st.is_deleted)
                .OrderByDescending(st => st.created_at)
                .ToList();
        }

        public List<support_ticket> GetTicketsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.support_tickets
                .Include(st => st.User)
                .Where(st => st.created_at >= startDate && st.created_at <= endDate && !st.is_deleted)
                .OrderByDescending(st => st.created_at)
                .ToList();
        }

        public void SoftDelete(string ticketId)
        {
            var ticket = _context.support_tickets.Find(ticketId);
            if (ticket != null)
            {
                ticket.is_deleted = true;
            }
        }

        public void ResolveTicket(string ticketId, string resolvedBy)
        {
            var ticket = _context.support_tickets.Find(ticketId);
            if (ticket != null)
            {
                ticket.status = "Resolved";
                ticket.resolved_at = DateTime.UtcNow;
                ticket.resolved_by = resolvedBy;
            }
        }
    }
}
