using Final_project.Models;

namespace Final_project.Repository.NotificationRepoFile
{
    public class NotificationRepo : INotificationRepo
    {
        private readonly AmazonDBContext db;

        public NotificationRepo(AmazonDBContext db)
        {
            this.db = db;
        }
        public void add(notification entity)
        {
            db.Notifications.Add(entity);
        }

        public List<notification> getAll()
        {
            throw new NotImplementedException();
        }

        public notification getById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(notification entity)
        {
            throw new NotImplementedException();
        }
    }
}
