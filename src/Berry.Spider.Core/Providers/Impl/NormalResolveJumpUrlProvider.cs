namespace Berry.Spider.Core;

public class NormalResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    public Task<string> ResolveAsync(string sourceUrl)
    {
        return Task.FromResult<string>(sourceUrl);
    }
}