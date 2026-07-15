using System.Net;
using System.Text.Json;

namespace XClone.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 1. По умолчанию ставим статус 500 (Server Error)
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Произошла внутренняя ошибка сервера.";
        
        switch (exception)
        {
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest; // 400
                message = exception.Message;            // Текст из сервиса (например, "Email занят")
                break;
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;   // 404
                message = exception.Message;            // "Пользователь не найден"
                break;
            // Сюда можно добавлять другие специфичные ошибки (например, UnauthorizedException)
        }
        

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        
        var response = JsonSerializer.Serialize(new { Error = message });
        
        return context.Response.WriteAsync(response);
    }
}