using Final_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Final_project.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AmazonDBContext _context;
        public UserRepository(AmazonDBContext context) { _context = context; }
        public IQueryable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>> filter = null, params Expression<Func<ApplicationUser, object>>[] includes)
        {
            IQueryable<ApplicationUser> query = _context.Users;
            if (filter != null)
                query = query.Where(filter);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return query;
        }
        public async Task<ApplicationUser> GetAsync(Expression<Func<ApplicationUser, bool>> filter, params Expression<Func<ApplicationUser, object>>[] includes)
        {
            return await GetAll(filter, includes).FirstOrDefaultAsync();
        }
        public async Task<ApplicationUser> GetByIdAsync(string id) => await _context.Users.FindAsync(id);
        public async Task<int> GetCountAsync(Expression<Func<ApplicationUser, bool>> filter = null)
        {
            if (filter != null)
                return await _context.Users.CountAsync(filter);
            return await _context.Users.CountAsync();
        }
        public async Task AddAsync(ApplicationUser entity) => await _context.Users.AddAsync(entity);
        public void add(ApplicationUser entity) => _context.Users.Add(entity);
        public void Update(ApplicationUser entity) => _context.Users.Update(entity);
        public void Delete(ApplicationUser entity) => _context.Users.Remove(entity);

        public List<ApplicationUser> getAll()
        {
            throw new NotImplementedException();
        }

        public ApplicationUser getById(string id)
        {
            return _context.Users.SingleOrDefault(u => u.Id == id);
        }


    }
}