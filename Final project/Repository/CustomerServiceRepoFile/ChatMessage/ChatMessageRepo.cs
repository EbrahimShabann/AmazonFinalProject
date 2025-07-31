using Final_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Repository.CustomerServiceRepoFile.ChatMessage
{
    public class ChatMessageRepo : IChatMessageRepo
    {
        private readonly AmazonDBContext _context;

        public ChatMessageRepo(AmazonDBContext context)
        {
            _context = context;
        }

        public void add(chat_message entity)
        {
            entity.id = Guid.NewGuid().ToString();
            _context.chat_messages.Add(entity);
        }

        public List<chat_message> getAll()
        {
            return _context.chat_messages
                .Include(cm => cm.Sender)
                .OrderBy(cm => cm.sent_at)
                .ToList();
        }

        public chat_message getById(string id)
        {
            return _context.chat_messages
                .Include(cm => cm.Sender)
                .FirstOrDefault(cm => cm.id == id);
        }


        public chat_message GetLatestMessage(string sessionId)
        {
            return _context.chat_messages
                .Include(cm => cm.Sender)
                .Where(cm => cm.session_id == sessionId)
                .OrderByDescending(cm => cm.sent_at)
                .FirstOrDefault();
        }


        public List<chat_message> GetMessagesBySessionId(string sessionId)
        {
            return _context.chat_messages
                .Include(cm => cm.Sender)
                .Where(cm => cm.session_id == sessionId)
                .OrderBy(cm => cm.sent_at)
                .ToList();
        }


        public int GetUnreadMessageCount(string sessionId, string userId)
        {
            return _context.chat_messages
                .Count(cm => cm.session_id == sessionId && cm.sender_id != userId && cm.is_read == false);
        }

        public List<chat_message> GetUnreadMessages(string sessionId, string userId)
        {
            return _context.chat_messages
                .Include(cm => cm.Sender)
                .Where(cm => cm.session_id == sessionId && cm.sender_id != userId && cm.is_read == false)
                .OrderBy(cm => cm.sent_at)
                .ToList();
        }


        public void MarkMessagesAsRead(string sessionId, string userId)
        {
            var unreadMessages = _context.chat_messages
                .Where(cm => cm.session_id == sessionId && cm.sender_id != userId && cm.is_read == false)
                .ToList();

            foreach (var message in unreadMessages)
            {
                message.is_read = true;
            }
        }

        public void Update(chat_message entity)
        {
            _context.chat_messages.Update(entity);
        }
    }
}
