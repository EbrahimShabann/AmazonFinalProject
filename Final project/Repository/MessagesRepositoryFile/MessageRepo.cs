using Final_project.Models;

namespace Final_project.Repository.MessagesRepositoryFile
{
    public class MessageRepo : IMessagesRepo
    {
        private readonly AmazonDBContext db;

        public MessageRepo(AmazonDBContext db)
        {
            this.db = db;
        }
        public void add(chat_message entity)
        {
            throw new NotImplementedException();
        }

        public List<chat_message> getAll()
        {
           return db.chat_messages.ToList();
        }

        public chat_message getById(string id)
        {
            throw new NotImplementedException();
        }

        public List<chat_message> getBySenderId(string senderId)
        {
            return getAll().Where(c=>c.sender_id==senderId).ToList();
        }

        public void Update(chat_message entity)
        {
            throw new NotImplementedException();
        }
    }
}
