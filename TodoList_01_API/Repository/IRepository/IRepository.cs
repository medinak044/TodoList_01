using System.Linq.Expressions;

namespace TodoList_01_API.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate); // PostgreSQL compatible
    //IEnumerable<T> GetSome(Expression<Func<T, bool>> predicate);
    IEnumerable<T> GetSome(Func<T, bool> predicate);
    Task<bool> AddAsync(T entity); // returns void to separate Save() logic
    Task<bool> RemoveAsync(T entity);
    Task<bool> RemoveRangeAsync(IEnumerable<T> entity);
    Task<bool> UpdateAsync(T entity);
    bool UpdateRange(IEnumerable<T> entities);
}