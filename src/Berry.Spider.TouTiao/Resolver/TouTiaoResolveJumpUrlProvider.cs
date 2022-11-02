using Berry.Spider.Core;
using System.Web;

namespace Berry.Spider.TouTiao;

public class TouTiaoResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    public Task<string> ResolveAsync(string sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl)) return Task.FromResult("");

        //去获取so.toutiao.com、tsearch.toutiaoapi.com的记录
        Uri sourceUri = new Uri(sourceUrl);
        //?url=https://so.toutiao.com/xxx
        string jumpUrl = sourceUri.Query.Substring(5);
        if (jumpUrl.StartsWith("http") || jumpUrl.StartsWith("https"))
        {
            Uri jumpUri = new Uri(HttpUtility.UrlDecode(jumpUrl));
            if (jumpUri.Host.Contains("toutiao"))
            {
                string url = jumpUri.ToString();
                return Task.FromResult<string>(url);
            }
        }

        return Task.FromResult("");
    }
}