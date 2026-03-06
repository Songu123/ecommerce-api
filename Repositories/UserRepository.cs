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

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _appContext.Users.Where(u => u.Email == email);

            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
