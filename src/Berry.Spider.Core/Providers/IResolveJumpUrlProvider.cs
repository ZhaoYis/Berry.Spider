namespace Berry.Spider.Core;

/// <summary>
/// 解析真实跳转的Url地址
/// </summary>
public interface IResolveJumpUrlProvider
{
    ValueTask<string> ResolveAsync(string sourceUrl);
}