using XClone.Api.DTOs;

namespace XClone.Api.Services;

public interface IUserService
{
    public Task ToggleFollowAsync(Guid followerId, Guid followeeId);
    public Task<UserProfileResponse> GetUserProfileAsync(string username);
}
