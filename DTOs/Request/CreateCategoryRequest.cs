using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    /// <summary>
    /// DTO for creating a new category
    /// </summary>
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Tên danh m?c là b?t bu?c")]
        [MaxLength(50, ErrorMessage = "Tên không ???c v??t quá 50 ký t?")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Mô t? không ???c v??t quá 200 ký t?")]
        public string? Description { get; set; }
    }
}
