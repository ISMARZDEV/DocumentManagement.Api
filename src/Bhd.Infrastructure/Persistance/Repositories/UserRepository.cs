using Bhd.Domain.Entities;
using Bhd.Domain.Interfaces;
using Bhd.Infrastructure.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Bhd.Infrastructure.Persistance.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User> AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.ToLower().Trim() == email.ToLower().Trim());
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == email.ToLower().Trim());
    }
}