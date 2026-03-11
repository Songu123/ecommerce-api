using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    /// <summary>
    /// Token request for refresh token flow
    /// </summary>
    public class TokenRequest
    {
        [Required(ErrorMessage = "Access token là bắt buộc")]
        public string Token  { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh token là bắt buộc")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
