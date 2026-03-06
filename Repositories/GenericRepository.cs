using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Repositories
{
    /// <summary>
    /// Generic Repository Implementation
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
   }

        public virtual async Task<T?> GetByIdAsync(int id)
    {
            return await _dbSet.FindAsync(id);
    }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
      {
 return await _dbSet.ToListAsync();
        }

  public virtual IQueryable<T> GetAll()
        {
      return _dbSet.AsQueryable();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
     {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
       int pageNumber,
            int pageSize,
        Expression<Func<T, bool>>? filter = null,
      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
 IQueryable<T> query = _dbSet;

      if (filter != null)
            {
 query = query.Where(filter);
      }

  var totalCount = await query.CountAsync();

          if (orderBy != null)
       {
query = orderBy(query);
            }

          var items = await query
       .Skip((pageNumber - 1) * pageSize)
 .Take(pageSize)
            .ToListAsync();

          return (items, totalCount);
        }

      public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            // Không g?i SaveChanges ? ?ây, ?? service layer quy?t ??nh
       return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
     await _dbSet.AddRangeAsync(entities);
       // Không g?i SaveChanges ? ?ây
    return entities;
  }

        public virtual async Task UpdateAsync(T entity)
        {
        _dbSet.Update(entity);
      // Không g?i SaveChanges ? ?ây
 await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(int id)
    {
            var entity = await GetByIdAsync(id);
            if (entity != null)
          {
    _dbSet.Remove(entity);
       // Không g?i SaveChanges ? ?ây
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
     _dbSet.Remove(entity);
      // Không g?i SaveChanges ? ?ây
            await Task.CompletedTask;
        }

        public virtual async Task<int> SaveAsync()
    {
     return await _context.SaveChangesAsync();
     }

    public virtual async Task<bool> ExistsAsync(int id)
  {
            var entity = await GetByIdAsync(id);
    return entity != null;
}

 public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
    return await _dbSet.CountAsync();
}

            return await _dbSet.CountAsync(predicate);
        }
    }
}
