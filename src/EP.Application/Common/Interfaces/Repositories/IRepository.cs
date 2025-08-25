using System.Linq.Expressions;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(long id);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindManyAsTrackingAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
        Task<bool> CheckExist(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> SelectWithConditionAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, bool asNoTracking = true);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        public void UpdateRange(IEnumerable<T> entities);
        Task Remove(T entity);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector);
    }
}
