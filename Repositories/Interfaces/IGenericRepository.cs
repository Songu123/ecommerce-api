using System.Linq.Expressions;

namespace API.Repositories.Interfaces
{
    /// <summary>
    /// Generic Repository Interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        // Query
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAll(); // Tr? v? IQueryable ?? có th? Include, Where...
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
   
  // Pagination
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
    int pageSize,
            Expression<Func<T, bool>>? filter = null,
  Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

   // Command
        Task<T> AddAsync(T entity);
      Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
        
     // Save Changes
        Task<int> SaveAsync();
        
     // Check
    Task<bool> ExistsAsync(int id);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
