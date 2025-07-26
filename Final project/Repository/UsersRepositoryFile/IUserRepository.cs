using System.Linq.Expressions;
using Final_project.Models;
using Microsoft.EntityFrameworkCore;
namespace Final_project.Repository
{
    public interface IUserRepository : IRepository<ApplicationUser> {
        public IQueryable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>> filter = null, params Expression<Func<ApplicationUser, object>>[] includes);

        public Task<ApplicationUser> GetAsync(Expression<Func<ApplicationUser, bool>> filter, params Expression<Func<ApplicationUser, object>>[] includes);

        public  Task<ApplicationUser> GetByIdAsync(string id) ;
        public Task<int> GetCountAsync(Expression<Func<ApplicationUser, bool>> filter = null);

        public  Task AddAsync(ApplicationUser entity) ;
        public void Delete(ApplicationUser entity);
    }
} 