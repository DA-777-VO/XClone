using XClone.Api.DTOs;
using XClone.Api.Entities;

namespace XClone.Api.Services;

public interface IUserService
{
    public Task ToggleFollowAsync(Guid followerId, Guid followeeId);
    public Task<UserProfileResponse> GetUserProfileAsync(string username);
    public Task UpdateProfileAsync(Guid userId, string? bio);

    public Task<UserProfileResponse> GetUserProfileByIdAsync(Guid id);
}
