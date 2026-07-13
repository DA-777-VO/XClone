using Microsoft.EntityFrameworkCore;
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

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Subscription?> GetSubscriptionAsync(Guid followerId, Guid followeeId)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(s => s.FollowerId == followerId && s.FolloweeId == followeeId);
    }

    public async Task AddSubscriptionAsync(Subscription subscription)
    {
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveSubscriptionAsync(Subscription subscription)
    {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
    }
}
