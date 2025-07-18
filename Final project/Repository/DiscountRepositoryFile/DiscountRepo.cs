using Final_project.Models;

namespace Final_project.Repository.DiscountRepositoryFile
{
    public class DiscountRepo : IDiscountRepo
    {
        private readonly AmazonDBContext db;

        public DiscountRepo(AmazonDBContext db)
        {
            this.db = db;
        }
        public void add(discount entity)
        {
            db.discounts.Add(entity);
        }

        public void delete(discount entity)
        {
            var discount = getById(entity.id);
            if (discount != null)
            {
                discount.is_deleted = true; // Soft delete
                Update(discount);
            }
        }

        public List<discount> getAll()
        {
           return db.discounts.Where(e => e.is_deleted != true).ToList();
        }

        public discount getById(string id)
        {
            return db.discounts
                .Where(e => e.is_deleted != true).FirstOrDefault(d=>d.id==id);
                
        }

        public void Update(discount entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
