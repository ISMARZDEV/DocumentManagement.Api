using Bhd.Domain.Entities;

namespace Bhd.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User> AddUserAsync(User user);
    Task<bool> ExistsEmailAsync(string email);
}
