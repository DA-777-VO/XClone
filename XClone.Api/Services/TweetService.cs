using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using XClone.Api.DTOs;
using XClone.Api.Entities;
using XClone.Api.Repositories;
using XClone.Api.Extensions;
using XClone.Api.Hubs;

namespace XClone.Api.Services;

public class TweetService : ITweetService
{
    private readonly ITweetRepository _tweetRepository;
    private readonly IDistributedCache _cache;
    private readonly IHubContext<TweetHub> _hubContext;

    public TweetService(ITweetRepository tweetRepository, IDistributedCache cache, IHubContext<TweetHub> hubContext)
    {
        _tweetRepository = tweetRepository;
        _cache = cache;
        _hubContext = hubContext;
    }

    public async Task<Tweet> CreateTweetAsync(string text, Guid userId)
    {
        if (text.Length > 280)
        {
            throw new ArgumentException("Tweet text cannot exceed 280 characters.");
        }
        Tweet tweet = new Tweet //TODO check TweetResponse
        {
            Id=Guid.NewGuid(),
            Text = text,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
        await _tweetRepository.Add(tweet);
        
        await _cache.RemoveRecordAsync("all_tweets");
        
        await _hubContext.Clients.All.SendAsync("ReceiveNewTweet", tweet);
        
        return tweet;
    }

    public async Task<List<TweetResponse>> GetAllTweets()
    {
        string cacheKey = "all_tweets";
        
        var cachedTweets = await _cache.GetRecordAsync<List<TweetResponse>>(cacheKey);
        
        if (cachedTweets != null)
        {
            return cachedTweets;
        };

        List<TweetResponse> tweetsFromDb = await _tweetRepository.GetAllAsync();
        
        await _cache.SetRecordAsync(cacheKey, tweetsFromDb, TimeSpan.FromMinutes(5));

        return tweetsFromDb;
    }

    public async Task<int> ToggleLikeAsync(Guid userId, Guid tweetId)
    {
        Tweet tweet = await _tweetRepository.GetTweetByIdAsync(tweetId);
        if (tweet == null)
        {
            throw new KeyNotFoundException("Твит не найден.");
        }

        Like? existingLike = await _tweetRepository.GetLikeAsync(userId, tweetId);
        if (existingLike != null)
        {
            await _tweetRepository.RemoveLikeAsync(existingLike);
        }
        else
        {
            Like newLike = new Like
            {
                UserId = userId,
                TweetId = tweetId,
                CreatedAt = DateTime.UtcNow,
            };
            await _tweetRepository.AddLikeAsync(newLike);
        }
        
        await _cache.RemoveRecordAsync($"feed_{userId}");
        await _cache.RemoveRecordAsync("all_tweets");
        
        return await _tweetRepository.GetLikesCountAsync(tweetId);
    }

    public async Task<List<TweetResponse>> GetHomeFeedAsync(Guid userId)
    {

        string cacheKey = $"feed_{userId}";
        var cachedFeed = await _cache.GetRecordAsync<List<TweetResponse>>(cacheKey);

        if (cachedFeed != null)
        {
            return cachedFeed;
        }

        List<TweetResponse> feedFromDb = await _tweetRepository.GetHomeFeedAsync(userId);
        
        await _cache.SetRecordAsync(cacheKey, feedFromDb, TimeSpan.FromMinutes(5));

        return feedFromDb;
    }
}
