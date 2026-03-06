using API.DTOs.Request;
using API.DTOs.Response;
using API.Helpers;

namespace API.Services.Interfaces
{
    /// <summary>
    /// Product service interface
    /// </summary>
public interface IProductService
    {
 Task<PaginatedResponse<ProductResponse>> GetAllProductsAsync(int page, int pageSize);
     Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductResponse>> SearchProductsAsync(string keyword);
   Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request);
     Task<bool> DeleteProductAsync(int id);
    }
}
