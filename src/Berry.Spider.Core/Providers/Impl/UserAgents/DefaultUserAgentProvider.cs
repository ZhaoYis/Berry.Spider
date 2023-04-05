namespace Berry.Spider.Core;

public class DefaultUserAgentProvider : IUserAgentProvider
{
    /// <summary>
    /// 随机从User-Agent池中获取一个User-Agent
    /// </summary>
    /// <returns></returns>
    public Task<string> GetOnesAsync()
    {
        return Task.FromResult(UserAgentPoolHelper.RandomGetOne());
    }
}