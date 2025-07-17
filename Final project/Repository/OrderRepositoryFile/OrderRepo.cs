namespace Final_project.Repository.OrderRepositoryFile
{
    public class OrderRepo : IOrderRepo
    {
        private readonly dbContext db;

        public OrderRepo(dbContext db)
        {
            this.db = db;
        }
        public void add(Order entity)
        {
            db.Orders.Add(entity);
        }

        public void delete(Order entity)
        {
            var order= getById(entity.OrderId);
            order.IsDeleted = true; // Soft delete
            Update(order);
        }

        public List<Order> getAll()
        {
           return db.Orders.Where(e => e.IsDeleted != true).ToList();
        }

        public Order getById(int id)
        {
            db.Orders
                .Where(e => e.IsDeleted != true && e.OrderId == id);
        }

        public void Update(Order entity)
        {
            db.entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
