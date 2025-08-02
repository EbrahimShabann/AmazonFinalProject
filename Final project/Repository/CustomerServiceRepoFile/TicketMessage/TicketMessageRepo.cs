using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CustomerServiceRepoFile.TicketMessage
{
    public class TicketMessageRepo : ITicketMessageRepo
    {
        private readonly AmazonDBContext _context;

        public TicketMessageRepo(AmazonDBContext context)
        {
            _context = context;
        }
        public void add(ticket_message entity)
        {
            entity.id = Guid.NewGuid().ToString();
            _context.ticket_messages.Add(entity);
        }

        public List<ticket_message> getAll()
        {
            return _context.ticket_messages
                .Include(tm => tm.Sender)
                .OrderBy(tm => tm.sent_at)
                .ToList();
        }

        public ticket_message getById(string id)
        {
            return _context.ticket_messages
                .Include(tm => tm.Sender)
                .FirstOrDefault(tm => tm.id == id);
        }

        public void Update(ticket_message entity)
        {
            _context.ticket_messages.Update(entity);
        }

        public List<ticket_message> GetMessagesByTicketId(string ticketId)
        {
            return _context.ticket_messages
                .Include(tm => tm.Sender)
                .Where(tm => tm.ticket_id == ticketId)
                .OrderBy(tm => tm.sent_at)
                .ToList();
        }

        public List<ticket_message> GetUnreadMessages(string ticketId)
        {
            return _context.ticket_messages
                .Include(tm => tm.Sender)
                .Where(tm => tm.ticket_id == ticketId && tm.is_read == false)
                .OrderBy(tm => tm.sent_at)
                .ToList();
        }

        public void MarkMessagesAsRead(string ticketId, string userId)
        {
            var unreadMessages = _context.ticket_messages
                .Where(tm => tm.ticket_id == ticketId && tm.sender_id != userId && tm.is_read == false)
                .ToList();

            foreach (var message in unreadMessages)
            {
                message.is_read = true;
            }
        }

        public ticket_message GetLatestMessage(string ticketId)
        {
            return _context.ticket_messages
                .Include(tm => tm.Sender)
                .Where(tm => tm.ticket_id == ticketId)
                .OrderByDescending(tm => tm.sent_at)
                .FirstOrDefault();
        }
    }
}
