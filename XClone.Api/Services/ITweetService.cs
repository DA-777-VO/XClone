using XClone.Api.Entities;

namespace XClone.Api.Services;

public interface ITweetService
{
    public Task<Tweet> CreateTweetAsync(string text, Guid userId);
    public Task<List<Tweet>> GetAllTweets();
    public Task ToggleLikeAsync(Guid userId, Guid tweetId);
}