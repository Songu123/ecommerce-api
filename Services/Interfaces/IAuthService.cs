using API.DTOs.Request;
using API.DTOs.Response;
using System.Security.Claims;

namespace API.Services.Interfaces
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RefreshTokenAsync(TokenRequest request);
        Task<bool> ValidateTokenAsync(string token);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    }
}
