using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using XClone.Api.Controllers;
using XClone.Api.Data;
using XClone.Api.Repositories;
using XClone.Api.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Мы меняем Singleton на Scoped, потому что работа с базой данных должна жить ровно один HTTP-запрос,
// чтобы вовремя закрывать подключение к базе и не перегружать память сервера.
builder.Services.AddScoped<ITweetRepository, EfTweetRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Сервис делаем Scoped (создается новый экземпляр на каждый HTTP-запрос). Это стандарт.
builder.Services.AddScoped<ITweetService, TweetService>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    // 1. Описываем схему авторизации (говорим Swagger, что мы используем JWT)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите токен в формате: Bearer {твой_токен}"
    });

    // 2. Требуем, чтобы Swagger прикреплял этот токен ко всем запросам, у которых есть замок [Authorize]
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// 1. Добавляем контекст БД в нашу коробку с инструментами (DI-контейнер)
builder.Services.AddDbContext<AppDbContext>(options =>
    // 2. Говорим использовать Postgres и передаем ему строку подключения, которую мы только что написали в appsettings.json
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}) .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidateAudience = true, 
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true,
        
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        // Превращаем наш секретный ключ из appsettings.json в байты для проверки подписи[[1](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQFQfbZFEwi07dzNLomUT_nd0YpY58kG2DHFCY3dlkx00HdxfLTnQx91S432KUyKUYu4uoJXvu1av96HdIACPY_TaVBLkF8TTsVbKDMyaZxd73vyM9j-ec3r8bbxh7EMFGveMCnfGeU%3D)][[5](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQGWje2FGsIe_k3mhzAlYURhCeuJDejegTF_AszU7SYbmSGxAGMLIrDg6s0TbglsrBxB3BLzuCZnH30wRtLtex7TuuhTP7hzrbacsu3OhivvPmyjG63Y9J4Hx0wMTfGoDJ2VYjcHkHyZVLprnOfum6R1gyB0DMOh-NjVtQtkleSxTnKXNTcX)]
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// app.UseHttpsRedirection(); // закомментировано для разработки — редирект ломает CORS

app.UseAuthorization();

app.MapControllers();

app.Run();