using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XClone.Api.DTOs;
using XClone.Api.Entities;
using XClone.Api.Services;

namespace XClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpPost("{followeeId}/follow")]
    public async Task<IActionResult> ToggleSubscription(Guid followeeId)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Console.WriteLine($"UserIdString: {userIdString}");

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("Пользователь не распознан.");
        }

        Guid followerId = Guid.Parse(userIdString);

        // Передаем в сервис ID подписчика и ID того, на кого подписываемся
        await _userService.ToggleFollowAsync(followerId, followeeId);

        return Ok("Подписка успешно изменена");
    }


    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        Guid userGuid = Guid.Parse(userId);

        UserProfileResponse profile = await _userService.GetUserProfileByIdAsync(userGuid);
        return Ok(profile);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserProfile(string username)
    {
        UserProfileResponse userProfile = await _userService.GetUserProfileAsync(username);
        return Ok(userProfile);
    }

    [Authorize]
    [HttpPut("profile")] 
    public async Task<IActionResult> UpdateProfileBio([FromBody] UpdateProfileRequest upadatedBio)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized();
        }
        Guid userId = Guid.Parse(userIdString);

        await _userService.UpdateProfileAsync(userId, upadatedBio?.Bio);
        return Ok("profile updated");
    }
}
