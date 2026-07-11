using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using XClone.Api.Entities;
using XClone.Api.Repositories;

namespace XClone.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    
    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        // 2. Берем секретный ключ из appsettings.json и превращаем его в байты [1]
        var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var key = new SymmetricSecurityKey(keyBytes);

        // 3. Создаем цифровую подпись с использованием алгоритма HMAC-SHA256
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Собираем токен: указываем кто издатель, для кого, данные пользователя, 
        // время жизни (например, 1 день) и нашу подпись [1].
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1), // Токен сгорит через 24 часа
            signingCredentials: creds
        );

        // 5. Превращаем объект токена в финальную строку-абракадабру
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task Register(string username, string email, string password)
    {
        User? userEmail = await _userRepository.GetByEmailAsync(email);
        if (userEmail != null)
        {
            throw new ArgumentException("Email already in use.");
        }
        
        User? userUsername = await _userRepository.GetByUsernameAsync(username);
        if (userUsername != null)
        {
            throw new ArgumentException("Username already in use.");
        }
        
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        User newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };
        await _userRepository.AddUserAsync(newUser);
    }

    public async Task<string> Login(string username, string password)
    {
        User user = await _userRepository.GetByEmailAsync(username) ?? await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            throw new ArgumentException("Неверный email или пароль.");
        }
        bool isPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isPassword)
        {
            throw new ArgumentException("Неверный email или пароль.");
        }
        return GenerateJwtToken(user);
    }
}