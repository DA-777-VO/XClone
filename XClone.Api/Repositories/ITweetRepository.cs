using XClone.Api.DTOs;
using XClone.Api.Entities;

namespace XClone.Api.Repositories;

public interface ITweetRepository
{
    Task Add(Tweet tweet);
    Task<List<Tweet>> GetAllAsync();
    
    Task AddLikeAsync(Like like);
    Task RemoveLikeAsync(Like like);
    Task<Like?> GetLikeAsync(Guid userId, Guid tweetId);
    Task<Tweet?> GetTweetByIdAsync(Guid id);
    Task<List<TweetResponse>> GetHomeFeedAsync(Guid userId);
}