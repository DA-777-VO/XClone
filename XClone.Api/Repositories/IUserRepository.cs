using XClone.Api.Entities;

namespace XClone.Api.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task<Subscription?> GetSubscriptionAsync(Guid followerId, Guid followeeId);
    Task AddSubscriptionAsync(Subscription subscription);
    Task RemoveSubscriptionAsync(Subscription subscription);
}