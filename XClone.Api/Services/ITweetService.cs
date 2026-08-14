using XClone.Api.DTOs;
using XClone.Api.Entities;

namespace XClone.Api.Services;

public interface ITweetService
{
    public Task<Tweet> CreateTweetAsync(string text, Guid userId);
    public Task<List<TweetResponse>> GetAllTweets();
    public Task<int> ToggleLikeAsync(Guid userId, Guid tweetId);
    public Task<List<TweetResponse>> GetHomeFeedAsync(Guid userId);
}