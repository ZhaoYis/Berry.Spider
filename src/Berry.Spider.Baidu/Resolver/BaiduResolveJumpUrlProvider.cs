using Berry.Spider.Core;

namespace Berry.Spider.Baidu;

public class BaiduResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    public ValueTask<string> ResolveAsync(string sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl)) return new ValueTask<string>("");

        if (sourceUrl.StartsWith("http") || sourceUrl.StartsWith("https"))
        {
            Uri jumpUri = new Uri(UrlHelper.UrlDecode(sourceUrl));
            if (jumpUri.Host.Contains("baidu"))
            {
                string url = jumpUri.ToString();
                return new ValueTask<string>(url);
            }
        }

        return new ValueTask<string>("");
    }
}