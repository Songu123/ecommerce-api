using API.Models;

namespace API.Repositories.Interfaces
{
    /// <summary>
  /// Product repository interface
    /// </summary>
    public interface IProductRepository : IGenericRepository<Product>
    {
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
     Task<IEnumerable<Product>> SearchProductsAsync(string keyword);
        Task<bool> IsProductNameExistsAsync(string name, int? excludeId = null);
    }
}
