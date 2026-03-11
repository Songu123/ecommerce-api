using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    /// <summary>
    /// User repository implementation
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _appContext;

        public UserRepository(AppDbContext context) : base(context)
        {
            _appContext = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _appContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(int userId)
        {
            return await _appContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _appContext.Users.Where(u => u.Email == email);

            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate)
        {
            // Revoke all existing refresh tokens for this user
            var existingTokens = await _appContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in existingTokens)
            {
                token.IsRevoked = true;
            }

            // Create new refresh token
            var newToken = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = expiryDate,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _appContext.RefreshTokens.Add(newToken);
            await _appContext.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(int userId)
        {
            var token = await _appContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .OrderByDescending(rt => rt.CreatedAt)
                .FirstOrDefaultAsync();

            if (token != null)
            {
                token.IsRevoked = true;
                await _appContext.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserRefreshTokensAsync(int userId)
        {
            var tokens = await _appContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _appContext.SaveChangesAsync();
        }
    }
}
