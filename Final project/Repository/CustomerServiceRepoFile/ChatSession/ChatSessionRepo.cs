using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CustomerServiceRepoFile.ChatSession
{
    public class ChatSessionRepo : IChatSessionRepo
    {
        private readonly AmazonDBContext _context;

        public ChatSessionRepo(AmazonDBContext context)
        {
            _context = context;
        }
        public void add(chat_session entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            _context.chat_sessions.Add(entity);
        }

        public List<chat_session> getAll()
        {
            return _context.chat_sessions
                .Include(cs => cs.Customer)
                .Include(cs => cs.Seller)
                .Where(cs => !cs.IsDeleted)
                .OrderByDescending(cs => cs.LastMessageAt)
                .ToList();
        }

        public chat_session getById(string id)
        {
            return _context.chat_sessions
                .Include(cs => cs.Customer)
                .Include(cs => cs.Seller)
                .FirstOrDefault(cs => cs.Id == id && !cs.IsDeleted);
        }
        public void Update(chat_session entity)
        {
            _context.chat_sessions.Update(entity);
        }
        public List<chat_session> GetSessionsByCustomerId(string customerId)
        {
            return _context.chat_sessions
                .Include(cs => cs.Customer)
                .Include(cs => cs.Seller)
                .Where(cs => cs.CustomerId == customerId && !cs.IsDeleted)
                .OrderByDescending(cs => cs.LastMessageAt)
                .ToList();
        }

        public List<chat_session> GetSessionsBySellerId(string sellerId)
        {
            return _context.chat_sessions
                .Include(cs => cs.Customer)
                .Include(cs => cs.Seller)
                .Where(cs => cs.SellerId == sellerId && !cs.IsDeleted)
                .OrderByDescending(cs => cs.LastMessageAt)
                .ToList();
        }
        public List<chat_session> GetActiveSessions()
        {
            return _context.chat_sessions
                .Include(cs => cs.Customer)
                .Include(cs => cs.Seller)
                .Where(cs => cs.Status == "Active" && !cs.IsDeleted)
                .OrderByDescending(cs => cs.LastMessageAt)
                .ToList();
        }



        public chat_session GetSessionByParticipants(string customerId, string sellerId)
        {
            return _context.chat_sessions
                .Include(cs => cs.Customer)
                .Include(cs => cs.Seller)
                .FirstOrDefault(cs => cs.CustomerId == customerId && cs.SellerId == sellerId && !cs.IsDeleted);
        }

        public void CloseSession(string sessionId)
        {
            var session = _context.chat_sessions.Find(sessionId);
            if (session != null)
            {
                session.Status = "Closed";
                session.ClosedAt = DateTime.UtcNow;
            }
        }

        public void UpdateLastMessageTime(string sessionId)
        {
            var session = _context.chat_sessions.Find(sessionId);
            if (session != null)
            {
                session.LastMessageAt = DateTime.UtcNow;
            }
        }


    }
}
