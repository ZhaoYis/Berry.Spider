using FreeRedis;
using Microsoft.Extensions.Options;

namespace Berry.Spider.FreeRedis;

public class RedisService : IRedisService
{
    private readonly RedisClient _redisClient;

    public RedisService(IOptionsSnapshot<RedisOptions> options)
    {
        if (options.Value.IsEnabled)
        {
            _redisClient = new RedisClient(options.Value.Configuration);
        }
        else
        {
            throw new Exception("未启用Redis...");
        }
    }

    public async Task<bool> SetAsync<T>(string key, T source)
    {
        long result = await _redisClient.SAddAsync(key, source);
        return result > 0;
    }

    public async Task<bool> SetAsync<T>(string key, T[] source)
    {
        long result = await _redisClient.SAddAsync(key, source);
        return result > 0;
    }

    public async Task<T[]> GetAllAsync<T>(string key)
    {
        T[] result = await _redisClient.SMembersAsync<T>(key);
        return result;
    }

    public async Task<bool> HSetAsync<T>(string key, string field, T value)
    {
        long result = await _redisClient.HSetAsync(key, field, value);
        return result > 0;
    }

    public async Task<T> HGetAsync<T>(string key, string field)
    {
        var result = await _redisClient.HGetAsync<T>(key, field);
        return result;
    }

    public async Task<Dictionary<string, T>> HGetAllAsync<T>(string key)
    {
        var result = await _redisClient.HGetAllAsync<T>(key);
        return result;
    }
}