using API.DTOs.Request;
using API.DTOs.Response;
using API.Helpers;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    /// <summary>
    /// Product service implementation
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
     private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepo,
  IMapper mapper,
   ILogger<ProductService> logger)
 {
      _productRepo = productRepo;
    _mapper = mapper;
   _logger = logger;
        }

        public async Task<PaginatedResponse<ProductResponse>> GetAllProductsAsync(int page, int pageSize)
     {
            var query = _productRepo.GetAll().Include(p => p.Category);

   var totalCount = await query.CountAsync();
     var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

  var products = await query
    .OrderByDescending(p => p.CreatedAt)
         .Skip((page - 1) * pageSize)
   .Take(pageSize)
   .ToListAsync();

    var productResponses = _mapper.Map<List<ProductResponse>>(products);

  return new PaginatedResponse<ProductResponse>
    {
     Items = productResponses,
     CurrentPage = page,
      TotalPages = totalPages,
        PageSize = pageSize,
       TotalCount = totalCount
     };
}

 public async Task<ProductResponse?> GetProductByIdAsync(int id)
   {
     var product = await _productRepo.GetAll()
   .Include(p => p.Category)
       .FirstOrDefaultAsync(p => p.Id == id);

      return product != null ? _mapper.Map<ProductResponse>(product) : null;
}

      public async Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId)
  {
    var products = await _productRepo.GetProductsByCategoryAsync(categoryId);
   return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

 public async Task<IEnumerable<ProductResponse>> SearchProductsAsync(string keyword)
        {
            var products = await _productRepo.SearchProductsAsync(keyword);
         return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
   {
   // Validate product name uniqueness
            if (await _productRepo.IsProductNameExistsAsync(request.Name))
   {
 throw new InvalidOperationException("Tên s?n ph?m ?ã t?n t?i");
  }

   var product = _mapper.Map<Product>(request);
     product.CreatedAt = DateTime.UtcNow;

     // TODO: Handle image upload
  // if (request.Image != null)
 // {
   //     product.ImageUrl = await _fileService.SaveFileAsync(request.Image);
     // }

    await _productRepo.AddAsync(product);
  await _productRepo.SaveAsync();

     return _mapper.Map<ProductResponse>(product);
    }

        public async Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request)
  {
    var product = await _productRepo.GetByIdAsync(id);
   if (product == null)
            {
       throw new KeyNotFoundException($"Product v?i ID {id} không t?n t?i");
  }

 // Validate product name uniqueness (exclude current product)
     if (await _productRepo.IsProductNameExistsAsync(request.Name, id))
  {
       throw new InvalidOperationException("Tên s?n ph?m ?ã t?n t?i");
  }

   _mapper.Map(request, product);
    product.UpdatedAt = DateTime.UtcNow;

   // TODO: Handle image upload
        // if (request.Image != null)
         // {
  //     product.ImageUrl = await _fileService.SaveFileAsync(request.Image);
    // }

  await _productRepo.UpdateAsync(product);
   await _productRepo.SaveAsync();

 return _mapper.Map<ProductResponse>(product);
        }

   public async Task<bool> DeleteProductAsync(int id)
        {
     var product = await _productRepo.GetByIdAsync(id);
     if (product == null)
     {
      return false;
     }

  await _productRepo.DeleteAsync(id);
  await _productRepo.SaveAsync();
  
            return true;
  }
    }
}
