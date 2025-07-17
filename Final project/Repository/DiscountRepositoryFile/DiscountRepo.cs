namespace Final_project.Repository.DiscountRepositoryFile
{
    public class DiscountRepo : IDiscountRepo
    {
        private readonly dbContext db;

        public DiscountRepo(dbContext db)
        {
            this.db = db;
        }
        public void add(Discount entity)
        {
            db.Discounts.Add(entity);
        }

        public void delete(Discount entity)
        {
            var discount = getById(entity.DiscountId);
            if (discount != null)
            {
                discount.IsDeleted = true; // Soft delete
                Update(discount);
            }
        }

        public List<Discount> getAll()
        {
           return db.Discounts.Where(e => e.IsDeleted != true).ToList();
        }

        public Discount getById(int id)
        {
            return db.Discounts
                .Where(e => e.IsDeleted != true && e.DiscountId == id);
                
        }

        public void Update(Discount entity)
        {
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
