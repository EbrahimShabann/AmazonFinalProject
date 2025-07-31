using Final_project.Models;

namespace Final_project.Repository.CustomerServiceRepoFile.ChatMessage
{
    public interface IChatMessageRepo : IRepository<chat_message>
    {
        List<chat_message> GetMessagesBySessionId(string sessionId);
        List<chat_message> GetUnreadMessages(string sessionId, string userId);
        void MarkMessagesAsRead(string sessionId, string userId);
        chat_message GetLatestMessage(string sessionId);
        int GetUnreadMessageCount(string sessionId, string userId);
    }
}
