using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    /// <summary>
    /// Register request DTO
    /// </summary>
    public class RegisterRequest
    {
        [Required(ErrorMessage = "H? tên là b?t bu?c")]
    [MaxLength(100, ErrorMessage = "H? tên không ???c v??t quá 100 ký t?")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là b?t bu?c")]
        [EmailAddress(ErrorMessage = "Email không ?úng ??nh d?ng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password là b?t bu?c")]
        [MinLength(6, ErrorMessage = "Password ph?i có ít nh?t 6 ký t?")]
  public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Password không kh?p")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
