using XClone.Api.Data;
using XClone.Api.Entities;

namespace XClone.Api.Repositories;

public class EfUserRepository : IUserRepository
{
    
    public readonly AppDbContext _context;
    
    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }
}