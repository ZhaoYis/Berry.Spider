namespace Berry.Spider.Core;

public class NormalResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    public ValueTask<string> ResolveAsync(string sourceUrl)
    {
        return new ValueTask<string>(sourceUrl);
    }
}