namespace XClone.Api.Services;

public interface IUserService
{
    public Task ToggleFollowAsync(Guid followerId, Guid followeeId);
}