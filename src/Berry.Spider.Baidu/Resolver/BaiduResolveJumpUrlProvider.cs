using Berry.Spider.Core;
using System.Web;

namespace Berry.Spider.Baidu;

public class BaiduResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    public Task<string> ResolveAsync(string sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl)) return Task.FromResult("");

        if (sourceUrl.StartsWith("http") || sourceUrl.StartsWith("https"))
        {
            Uri jumpUri = new Uri(HttpUtility.UrlDecode(sourceUrl));
            if (jumpUri.Host.Contains("baidu"))
            {
                string url = jumpUri.ToString();
                return Task.FromResult<string>(url);
            }
        }

        return Task.FromResult("");
    }
}