using API.Models;

namespace API.Repositories.Interfaces
{
    /// <summary>
    /// Category repository interface
  /// </summary>
    public interface ICategoryRepository : IGenericRepository<Category>
    {
     Task<Category?> GetCategoryWithProductsAsync(int id);
    Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null);
    }
}
