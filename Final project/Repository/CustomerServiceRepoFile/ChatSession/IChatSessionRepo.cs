using Final_project.Models;

namespace Final_project.Repository.CustomerServiceRepoFile.ChatSession
{
    public interface IChatSessionRepo:IRepository<chat_session>
    {
        List<chat_session> GetSessionsByCustomerId(string customerId);
        List<chat_session> GetSessionsBySellerId(string sellerId);
        List<chat_session> GetActiveSessions();
        chat_session GetSessionByParticipants(string customerId, string sellerId);
        void CloseSession(string sessionId);
        void UpdateLastMessageTime(string sessionId);
    }
}
