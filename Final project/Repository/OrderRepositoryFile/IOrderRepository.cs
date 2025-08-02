using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public interface IOrderRepository : IRepository<order>
    {
        public IQueryable<order> GetAll(Expression<Func<order, bool>> filter = null, params Expression<Func<order, object>>[] includes);
        public  Task<order> GetAsync(Expression<Func<order, bool>> filter, params Expression<Func<order, object>>[] includes);

        public  Task<order> GetByIdAsync(string id);
        public  Task<int> GetCountAsync(Expression<Func<order, bool>> filter = null);

        public Task AddAsync(order entity);
        public void Delete(order entity);




    }
} 