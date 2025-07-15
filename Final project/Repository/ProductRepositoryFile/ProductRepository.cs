using Final_project.Models;
using Final_project.Repository.Product;

namespace Final_project.Repository.ProductRepositoryFile
{
    public class ProductRepository : IProductRepository
    {
        //private readonly dbContext db;

        //public ProductRepository(dbContext db)
        //{
        //    this.db = db;
        //}
        //public void add(product entity)
        //{
        //    db.Products.Add(entity);
        //}
        ////SoftDelete
        //public void delete(product entity)
        //{
        //    var data = getById(entity.ProductId);
        //    if (data != null)
        //    {
        //        data.IsDeleted = true;
        //        Update(data);
        //    }
        //}
        ////get all exipt the deleted ones
        //public List<product> getAll()
        //{
        //   return db.Set<product>().Where(e=>e.IsDeleted!=true).ToList();
        //}
        ////get product adn it's not deleted
        //public product getById(int id)
        //{
        //    return db.Set<product>()
        //          .Where(e => e.IsDeleted != true)
        //          .FirstOrDefault(e=>e.ProductId==id);
        //}

        //public void Update(product entity)
        //{
        //    db.Entry(entity).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        //}
        public void add(product entity)
        {
            throw new NotImplementedException();
        }

        public void delete(product entity)
        {
            throw new NotImplementedException();
        }

        public List<product> getAll()
        {
            throw new NotImplementedException();
        }

        public product getById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(product entity)
        {
            throw new NotImplementedException();
        }
    }
}
