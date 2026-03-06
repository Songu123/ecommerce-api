using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.Interfaces
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
  Task<AuthResponse> RegisterAsync(RegisterRequest request);
     Task<bool> ValidateTokenAsync(string token);
    }
}
