using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XClone.Api.Services;

namespace XClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase
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
        try
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
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }

}
