using API.DTOs.Request;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Categories API Controller
    /// </summary>
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
    ICategoryService categoryService,
    ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of all categories</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return SuccessResponse(categories);
        }

        /// <summary>
        /// Get category by ID with products
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details with products</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
                return NotFoundResponse($"Không tìm th?y danh m?c v?i ID {id}");

            return SuccessResponse(category);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="request">Category creation data</param>
        /// <returns>Created category</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("D? li?u không h?p l?", ModelState.Values
                  .SelectMany(v => v.Errors)
             .Select(e => e.ErrorMessage)
                  .ToList());

            try
            {
                var category = await _categoryService.CreateCategoryAsync(request);
                return SuccessResponse(category, "T?o danh m?c thành công");
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="request">Category update data</param>
        /// <returns>Updated category</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("D? li?u không h?p l?", ModelState.Values
                 .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                    .ToList());

            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, request);
                return SuccessResponse(category, "C?p nh?t danh m?c thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (!result)
                    return NotFoundResponse($"Không tìm th?y danh m?c v?i ID {id}");

                return SuccessResponse(result, "Xóa danh m?c thành công");
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse(ex.Message);
            }
        }
    }
}
