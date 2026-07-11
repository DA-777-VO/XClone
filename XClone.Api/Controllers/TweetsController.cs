using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using XClone.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace XClone.Api.Controllers;

// Класс-помощник, чтобы ASP.NET красиво прочитал JSON, который мы ему пришлем
public class CreateTweetRequest
{
    public string Text { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class TweetsController : ControllerBase
{
    private readonly ITweetService _tweetService;

    public TweetsController(ITweetService tweetService)
    {
        _tweetService = tweetService;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetAllTweets()
    {
        var tweets = await _tweetService.GetAllTweets();
        return Ok(tweets);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTweet([FromBody] CreateTweetRequest request)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("Пользователь не распознан.");
            }

            Guid userId = Guid.Parse(userIdString);

            // Передаем в сервис текст твита и ID его автора
            await _tweetService.CreateTweetAsync(request.Text, userId);
        
            return Ok("Твит успешно опубликован!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{tweetId}/like")]
    public async Task<IActionResult> ToggleLike(Guid tweetId)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("Пользователь не распознан.");
            }

            Guid userId = Guid.Parse(userIdString);

            await _tweetService.ToggleLikeAsync(userId, tweetId);
        
            return Ok("Лайк успешно изменен!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
}