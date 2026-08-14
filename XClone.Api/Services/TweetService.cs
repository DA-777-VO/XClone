using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using XClone.Api.DTOs;
using XClone.Api.Entities;
using XClone.Api.Repositories;
using XClone.Api.Extensions;

namespace XClone.Api.Services;

public class TweetService : ITweetService
{
    private readonly ITweetRepository _tweetRepository;
    private readonly IDistributedCache _cache;

    public TweetService(ITweetRepository tweetRepository, IDistributedCache cache)
    {
        _tweetRepository = tweetRepository;
        _cache = cache;
    }

    public async Task<Tweet> CreateTweetAsync(string text, Guid userId)
    {
        if (text.Length > 280)
        {
            throw new ArgumentException("Tweet text cannot exceed 280 characters.");
        }
        Tweet tweet = new Tweet
        {
            Id=Guid.NewGuid(),
            Text = text,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
        await _tweetRepository.Add(tweet);
        
        await _cache.RemoveRecordAsync("all_tweets");
       
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

    public async Task ToggleLikeAsync(Guid userId, Guid tweetId)
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
            return;
        }

        Like newLike = new Like
        {
            UserId = userId,
            TweetId = tweetId,
            CreatedAt = DateTime.UtcNow,
        };
        await _tweetRepository.AddLikeAsync(newLike);
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
