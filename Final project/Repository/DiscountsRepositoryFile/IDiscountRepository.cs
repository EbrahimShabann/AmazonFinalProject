using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public interface IDiscountRepository : IRepository<discount>
    {
        public IQueryable<discount> GetAll(Expression<Func<discount, bool>> filter = null, params Expression<Func<discount, object>>[] includes);
        public Task<discount> GetAsync(Expression<Func<discount, bool>> filter, params Expression<Func<discount, object>>[] includes);
        public Task<discount> GetByIdAsync(string id);
        public Task<int> GetCountAsync(Expression<Func<discount, bool>> filter = null);
        public Task AddAsync(discount entity) ;
        public void Delete(discount entity);
        public void ApplyDiscountToProducts(discount entity, List<string> productIds);


    }
} 