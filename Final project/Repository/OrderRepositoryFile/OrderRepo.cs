using Final_project.Models;

namespace Final_project.Repository.OrderRepositoryFile
{
    public class OrderRepo : IOrderRepo
    {
        private readonly AmazonDBContext db;

        public OrderRepo(AmazonDBContext db)
        {
            this.db = db;
        }
        public void add(order entity)
        {
            db.orders.Add(entity);
        }

        public void addOrderItem(order_item entity)
        {
            db.order_items.Add(entity);
        }

        public List<order> getAll()
        {
            return db.orders.Where(o => !o.is_deleted).ToList();
        }

        public order getById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(order entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
