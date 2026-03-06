using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    /// <summary>
    /// Category repository implementation
    /// </summary>
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _appContext;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _appContext = context;
        }

        public async Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return await _appContext.Categories
          .Include(c => c.Products)
         .FirstOrDefaultAsync(c => c.Id == id);
        }

         public async Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null)
 {
            var query = _appContext.Categories.Where(c => c.Name == name);

          if (excludeId.HasValue)
            {
            query = query.Where(c => c.Id != excludeId.Value);
            }

         return await query.AnyAsync();
      }
    }
}
