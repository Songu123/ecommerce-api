using API.DTOs.Request;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Products API Controller
    /// </summary>
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
  IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetAllProductsAsync(page, pageSize);
            return PaginatedResponse(result);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFoundResponse($"Không tìm th?y s?n ph?m v?i ID {id}");

            return SuccessResponse(product);
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of products in category</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return SuccessResponse(products);
        }

        /// <summary>
        /// Search products by keyword
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <returns>List of matching products</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return ErrorResponse("T? khóa tìm ki?m không ???c ?? tr?ng");

            var products = await _productService.SearchProductsAsync(keyword);
            return SuccessResponse(products);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="request">Product creation data</param>
        /// <returns>Created product</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("D? li?u không h?p l?", ModelState.Values
               .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                .ToList());

            try
            {
                var product = await _productService.CreateProductAsync(request);
                return SuccessResponse(product, "T?o s?n ph?m thành công");
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="request">Product update data</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("D? li?u không h?p l?", ModelState.Values
            .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
            .ToList());

            try
            {
                var product = await _productService.UpdateProductAsync(id, request);
                return SuccessResponse(product, "C?p nh?t s?n ph?m thành công");
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
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result)
                return NotFoundResponse($"Không tìm th?y s?n ph?m v?i ID {id}");

            return SuccessResponse(result, "Xóa s?n ph?m thành công");
        }
    }
}
