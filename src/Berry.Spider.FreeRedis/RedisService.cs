using FreeRedis;
using Microsoft.Extensions.Options;

namespace Berry.Spider.FreeRedis;

public class RedisService : IRedisService
{
    private readonly RedisClient _redisClient = null;

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

    public async Task<bool> SetAsync(string key, string source)
    {
        long result = await _redisClient.SAddAsync(key, source);

        return result > 0;
    }

    public async Task<bool> SetAsync(string key, object[] source)
    {
        long result = await _redisClient.SAddAsync(key, source);

        return result > 0;
    }
}