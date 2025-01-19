using Berry.Spider.Core;

namespace Berry.Spider.Baidu;

public class BaiduResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    public Task<string> ResolveAsync(string sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl)) return Task.FromResult("");

        if (sourceUrl.StartsWith("http") || sourceUrl.StartsWith("https"))
        {
            Uri jumpUri = new Uri(UrlHelper.UrlDecode(sourceUrl));
            if (jumpUri.Host.Contains("baidu"))
            {
                string url = jumpUri.ToString();
                return Task.FromResult<string>(url);
            }
        }

        return Task.FromResult("");
    }
}