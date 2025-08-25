using EP.Application.Common.Interfaces.Repositories;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EP.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(Context context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindManyAsTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.AsNoTracking().Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> SelectWithConditionAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, bool asNoTracking = true)
        {
            if (asNoTracking)
            {
                return await _dbSet.AsNoTracking().Where(predicate).Select(selector).ToListAsync();
            }

            return await _dbSet.Where(predicate).Select(selector).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task Remove(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.AsNoTracking().MaxAsync(selector);
        }

        public async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.AsNoTracking().MinAsync(selector);
        }

        public async Task<bool> CheckExist(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
