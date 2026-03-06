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
    }
}
