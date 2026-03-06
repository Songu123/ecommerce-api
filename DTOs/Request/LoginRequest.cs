using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    /// <summary>
    /// Login request DTO
    /// </summary>
 public class LoginRequest
    {
   [Required(ErrorMessage = "Email là b?t bu?c")]
        [EmailAddress(ErrorMessage = "Email không ?úng ??nh d?ng")]
   public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password là b?t bu?c")]
        [MinLength(6, ErrorMessage = "Password ph?i có ít nh?t 6 ký t?")]
        public string Password { get; set; } = string.Empty;
    }
}
