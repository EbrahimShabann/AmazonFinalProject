using System.Collections.Generic;

namespace Final_project.Repository
{
    public interface IRepository<T> where T : class
    {
        public List<T> getAll();
        public T getById(string id);
        public void Update(T entity);
        public void add(T entity);
    }
}
