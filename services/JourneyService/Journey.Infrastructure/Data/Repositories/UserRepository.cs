using Journey.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Journey.Infrastructure.Data.Repositories;

public sealed class UserRepository : Repository<User, long>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task RemoveUser(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}