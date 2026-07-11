using XClone.Api.Entities;
using XClone.Api.Repositories;

namespace XClone.Api.Services;

public class TweetService : ITweetService
{
    private readonly ITweetRepository tweetRepository;

    public TweetService(ITweetRepository tweetRepository)
    {
        this.tweetRepository = tweetRepository;
    }
    
    public async Task CreateTweetAsync(string text, Guid userId)
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
        await tweetRepository.Add(tweet);
    }
    
    public async Task<List<Tweet>> GetAllTweets()
    {
        return await tweetRepository.GetAllAsync();
    }

    public async Task ToggleLikeAsync(Guid userId, Guid tweetId)
    {
        Tweet tweet = await tweetRepository.GetTweetByIdAsync(tweetId);
        if (tweet == null)
        {
            throw new KeyNotFoundException("Твит не найден.");
        }
        
        Like? existingLike = await tweetRepository.GetLikeAsync(userId, tweetId);
        if (existingLike != null)
        { 
            await tweetRepository.RemoveLikeAsync(existingLike);
            return;
        }

        Like newLike = new Like
        {
            UserId = userId,
            TweetId = tweetId,
            CreatedAt = DateTime.UtcNow,
        };
        await tweetRepository.AddLikeAsync(newLike);
    }
}