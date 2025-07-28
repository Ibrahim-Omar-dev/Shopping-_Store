using System.Linq.Expressions;

namespace DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        public T Get(int id);
       public void Add(T entity);
       public void Remove(T entity);
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        public void RemoveRange(IEnumerable<T> entity);

    }
}
