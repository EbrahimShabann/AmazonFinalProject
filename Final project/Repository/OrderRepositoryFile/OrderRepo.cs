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

        public void delete(order entity)
        {
            var order= getById(entity.id);
            order.is_deleted = true; // Soft delete
            Update(order);
        }

        public List<order> getAll()
        {
           return db.orders.Where(e => e.is_deleted != true).ToList();
        }

        public order getById(string id)
        {
            return db.orders
                .Where(e => e.is_deleted != true ).FirstOrDefault(o=>o.id==id);
        }

        public void Update(order entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
