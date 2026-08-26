using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XClone.Api.DTOs;
using XClone.Api.Entities;
using XClone.Api.Repositories;
using XClone.Api.Services;

namespace XClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFileService _fileService;
    private readonly IUserRepository _userRepository;

    public UserController(IUserService userService, IFileService fileService, IUserRepository userRepository)
    {
        _userService = userService;
        _fileService = fileService;
        _userRepository = userRepository;
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
    
    
    [Authorize]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file) // Без [FromBody]! Файлы так не передаются
    {
        // Базовые проверки безопасности
        if (file == null || file.Length == 0) return BadRequest("Файл не выбран");
        if (file.Length > 10 * 1024 * 1024) return BadRequest("Файл слишком большой (максимум 5 МБ)");

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        Guid userId = Guid.Parse(userIdString);

        // Генерируем уникальное имя файла (чтобы Вася не перезаписал аватарку Пети, если они оба загрузят "1.jpg")
        // Берем расширение из оригинального файла (например, .jpg или .png)
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"avatar_{userId}{extension}";

        // Отправляем файл в облако S3
        var fileUrl = await _fileService.UploadFileAsync(file, uniqueFileName);

        // Сохраняем ссылку в базу данных пользователя
        var user = await _userRepository.GetByIdAsync(userId);
        user.AvatarUrl = fileUrl;
        await _userRepository.UpdateAsync(user);

        return Ok(new { Message = "Аватарка успешно загружена", url = fileUrl });
    }
}
