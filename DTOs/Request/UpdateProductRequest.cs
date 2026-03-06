using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    /// <summary>
  /// DTO for updating an existing product
  /// </summary>
    public class UpdateProductRequest
    {
  [Required(ErrorMessage = "Tên s?n ph?m là b?t bu?c")]
   [MaxLength(100, ErrorMessage = "Tên s?n ph?m không ???c v??t quá 100 ký t?")]
    public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá là b?t bu?c")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá ph?i l?n h?n 0")]
  public decimal Price { get; set; }

[MaxLength(500, ErrorMessage = "Mô t? không ???c v??t quá 500 ký t?")]
        public string? Description { get; set; }

  [Range(0, int.MaxValue, ErrorMessage = "S? l??ng ph?i l?n h?n ho?c b?ng 0")]
   public int Stock { get; set; }

 [Required(ErrorMessage = "CategoryId là b?t bu?c")]
        public int CategoryId { get; set; }

        public IFormFile? Image { get; set; }
    }
}
