using API.DTOs.Request;
using API.DTOs.Response;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    /// <summary>
    /// Category service implementation
/// </summary>
    public class CategoryService : ICategoryService
    {
   private readonly ICategoryRepository _categoryRepo;
 private readonly IMapper _mapper;
   private readonly ILogger<CategoryService> _logger;

      public CategoryService(
     ICategoryRepository categoryRepo,
   IMapper mapper,
   ILogger<CategoryService> logger)
        {
       _categoryRepo = categoryRepo;
    _mapper = mapper;
       _logger = logger;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
       var categories = await _categoryRepo.GetAll()
    .Include(c => c.Products)
  .ToListAsync();

       return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
   }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepo.GetCategoryWithProductsAsync(id);
return category != null ? _mapper.Map<CategoryResponse>(category) : null;
        }

     public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        // Validate category name uniqueness
         if (await _categoryRepo.IsCategoryNameExistsAsync(request.Name))
     {
  throw new InvalidOperationException("Tên danh m?c ?ã t?n t?i");
  }

    var category = _mapper.Map<Category>(request);
  category.CreatedAt = DateTime.UtcNow;

  await _categoryRepo.AddAsync(category);
   await _categoryRepo.SaveAsync();

   return _mapper.Map<CategoryResponse>(category);
  }

     public async Task<CategoryResponse> UpdateCategoryAsync(int id, CreateCategoryRequest request)
  {
   var category = await _categoryRepo.GetByIdAsync(id);
   if (category == null)
    {
      throw new KeyNotFoundException($"Category v?i ID {id} không t?n t?i");
 }

   // Validate category name uniqueness (exclude current category)
if (await _categoryRepo.IsCategoryNameExistsAsync(request.Name, id))
     {
      throw new InvalidOperationException("Tên danh m?c ?ã t?n t?i");
}

     category.Name = request.Name;
    category.Description = request.Description;

            await _categoryRepo.UpdateAsync(category);
  await _categoryRepo.SaveAsync();

  return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
   var category = await _categoryRepo.GetCategoryWithProductsAsync(id);
if (category == null)
     {
    return false;
    }

 // Ki?m tra xem category có products không
      if (category.Products.Any())
            {
     throw new InvalidOperationException("Không th? xóa danh m?c ?ang có s?n ph?m");
  }

await _categoryRepo.DeleteAsync(id);
  await _categoryRepo.SaveAsync();

   return true;
        }
  }
}
