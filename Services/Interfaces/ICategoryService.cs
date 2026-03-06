using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.Interfaces
{
    /// <summary>
 /// Category service interface
 /// </summary>
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
 Task<CategoryResponse?> GetCategoryByIdAsync(int id);
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryResponse> UpdateCategoryAsync(int id, CreateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
