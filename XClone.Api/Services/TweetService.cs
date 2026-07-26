using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using XClone.Api.DTOs;
using XClone.Api.Entities;
using XClone.Api.Repositories;

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
        
        try
        {
            await _cache.RemoveAsync("all_tweets");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Failed to remove cache: {ex.Message}");
        }
        return tweet;
    }

    public async Task<List<TweetResponse>> GetAllTweets()
    {
        string cacheKey = "all_tweets";
        string? cachedTweets = null;

        try
        {
            cachedTweets = await _cache.GetStringAsync(cacheKey);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Redis is down: {ex.Message}. Falling back to Postgres.");
        }
        
        if (!string.IsNullOrEmpty(cachedTweets))
        {
            try
            {
                return JsonSerializer.Deserialize<List<TweetResponse>>(cachedTweets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Cache deserialization failed: {ex.Message}");
            }
        };

        List<TweetResponse> tweetsFromDb = await _tweetRepository.GetAllAsync();
        
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        
        try
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(tweetsFromDb), cacheOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Cache serialization failed: {ex.Message}");
        }

        return tweetsFromDb;
        // return await _tweetRepository.GetAllAsync();
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

        string? cachedFeed = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedFeed))
        {
            return JsonSerializer.Deserialize<List<TweetResponse>>(cachedFeed);
        }

        List<TweetResponse> feedFromDb = await _tweetRepository.GetHomeFeedAsync(userId);

        if (feedFromDb == null)
        {
            throw new KeyNotFoundException("User feed not found.");
        }

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(feedFromDb), cacheOptions);

        return feedFromDb;
    }
}
