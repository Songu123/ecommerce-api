using API.Models;

namespace API.Repositories.Interfaces
{
    /// <summary>
    /// User repository interface
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);

        Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate);
        Task<RefreshToken?> GetRefreshTokenAsync(int userId);
        Task RevokeRefreshTokenAsync(int userId);
        Task RevokeAllUserRefreshTokensAsync(int userId);
    }
}
