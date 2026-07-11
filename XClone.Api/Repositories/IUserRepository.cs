using XClone.Api.Entities;

namespace XClone.Api.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
}