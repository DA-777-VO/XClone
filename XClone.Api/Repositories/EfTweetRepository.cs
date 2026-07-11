using Microsoft.EntityFrameworkCore;
using XClone.Api.Data;
using XClone.Api.Entities;

namespace XClone.Api.Repositories;

public class EfTweetRepository : ITweetRepository
{
    private readonly AppDbContext _context;

    // Внедряем пульт управления базой данных (DbContext) через конструктор
    public EfTweetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Add(Tweet tweet)
    {
        _context.Tweets.Add(tweet); // Говорим контексту: "Запомни, этот твит нужно добавить"
        await _context.SaveChangesAsync();     // Сохраняем изменения. В этот момент EF Core отправляет в Postgres реальный SQL-запрос: INSERT INTO ...
    }

    public async Task<List<Tweet>> GetAllAsync()
    {
        // Достаем все записи из таблицы Tweets и превращаем в обычный список C# (это запрос SELECT * FROM)
        return  await _context.Tweets.ToListAsync();
    }

    public async Task<Like?> GetLikeAsync(Guid userId, Guid tweetId)
    {
        return await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.TweetId == tweetId );
    }

    public async Task<Tweet?> GetTweetByIdAsync(Guid id)
    {
        return await _context.Tweets.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task RemoveLikeAsync(Like like)
    {
        _context.Likes.Remove(like);
        await _context.SaveChangesAsync();
    }

    public async Task AddLikeAsync(Like like)
    {
        _context.Likes.Add(like);
        await _context.SaveChangesAsync();
    }
}