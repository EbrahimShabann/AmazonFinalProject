using Final_project.Models;

namespace Final_project.Repository.MessagesRepositoryFile
{
    public interface IMessagesRepo:IRepository<chat_message>
    {
        List<chat_message> getBySenderId(string senderId);
        void Delete(chat_message entity);
    }
}
