using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace XClone.Api.Extensions;

public static class DistributedCacheExtensions
{
    
    public static async Task SetRecordAsync<T>(
        this IDistributedCache cache, 
        string recordId, 
        T data, 
        TimeSpan? absoluteExpireTime = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(360),
        };

        try
        {
            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[REDIS WARNING] Не удалось записать кэш для ключа '{recordId}': {e.Message}");
        }
        
    }

    
    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        try
        {
            var jsonData = await cache.GetStringAsync(recordId);
            
            if (jsonData is null)
            {
                return default;
            }
            
            return JsonSerializer.Deserialize<T>(jsonData);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[REDIS WARNING] Не удалось прочитать кэш для ключа '{recordId}': {e.Message}");
            return default;
        }
        
        
    }

    public static async Task RemoveRecordAsync(this IDistributedCache cache, string recordId)
    {
        try
        {
            await cache.RemoveAsync(recordId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[REDIS WARNING] Не удалось удалить кэш для ключа '{recordId}': {ex.Message}");
            throw;
        }
    }
}