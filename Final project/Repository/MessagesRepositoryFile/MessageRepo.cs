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

        public void Delete(chat_message entity)
        {
            var message = getById(entity.id);
            if (message != null) 
            {
                entity.is_deleted = true;
                Update(entity);
            }
           
        }

        public List<chat_message> getAll()
        {
           return db.chat_messages.Where(m=>m.is_deleted != true).ToList();
        }

        public chat_message getById(string id)
        {
           return getAll().SingleOrDefault(m=>m.id== id && m.is_deleted != true);
        }

        public List<chat_message> getBySenderId(string senderId)
        {
            return getAll().Where(c=>c.sender_id==senderId).ToList();
        }

        public void Update(chat_message entity)
        {
            db.Entry(entity).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
