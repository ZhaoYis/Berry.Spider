using Volo.Abp.DependencyInjection;

namespace Berry.Spider.FreeRedis;

public interface IRedisService : ISingletonDependency
{
    Task<bool> SetAsync<T>(string key, T source);

    Task<bool> SetAsync<T>(string key, T[] source);

    Task<T[]> GetAllAsync<T>(string key);

    Task<bool> HSetAsync<T>(string key, string field, T value);

    Task<T> HGetAsync<T>(string key, string field);
    
    Task<bool> HDelAsync(string key, params string[] fields);

    Task<Dictionary<string, T>> HGetAllAsync<T>(string key);
}