using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    /// <summary>
    /// Product repository implementation
    /// </summary>
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _appContext;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _appContext = context;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _appContext.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
        {
            return await _appContext.Products
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(keyword) || 
                            (p.Description != null && p.Description.Contains(keyword)))
                .ToListAsync();
        }

        public async Task<bool> IsProductNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _appContext.Products.Where(p => p.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
