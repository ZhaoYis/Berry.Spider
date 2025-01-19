using System.Text.RegularExpressions;
using Berry.Spider.Core;
using Microsoft.Extensions.Options;

namespace Berry.Spider.TouTiao;

public class TouTiaoResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    private static Regex Toutiaoapi_Com_Regex = new(@"toutiaoapi\.com");
    private static Regex Toutiao_Com_Regex = new(@"toutiao\.com");

    private TouTiaoOptions TouTiaoOptions { get; }

    public TouTiaoResolveJumpUrlProvider(IOptionsSnapshot<TouTiaoOptions> options)
    {
        TouTiaoOptions = options.Value;
    }

    public Task<string> ResolveAsync(string sourceUrl)
    {
        string jumpToUrl = "";
        if (string.IsNullOrWhiteSpace(sourceUrl)) return Task.FromResult(jumpToUrl);

        //执行一次解码，结果范例如下：
        // /search/jump?url=https://tsearch.toutiaoapi.com/s/search_wenda/list?enable_miaozhen_page=1&enter_answer_id=7175162421706048012&enter_from=search_result&outer_show_aid=7175162421706048012&qid=6931623923998900488&relate_type=0&search_id=xxx&aid=4916&jtoken=xxx
        sourceUrl = UrlHelper.UrlDecode(sourceUrl);

        //只获取so.toutiao.com、tsearch.toutiaoapi.com的记录
        if (Toutiaoapi_Com_Regex.Match(sourceUrl).Success || Toutiao_Com_Regex.Match(sourceUrl).Success)
        {
            string pattern = this.TouTiaoOptions.JumpToPattern ?? @"\/search\/jump\?url=(.*)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(sourceUrl);
            if (match.Success)
            {
                jumpToUrl = match.Groups[1].Value;
            }
            else
            {
                //再做一次努力
                Regex latest = new Regex(@"=(.*)");
                if (latest.Match(sourceUrl).Success)
                {
                    jumpToUrl = match.Groups[1].Value;
                }
            }
        }

        if (!UrlHelper.IsUrl(jumpToUrl))
        {
            return Task.FromResult("");
        }

        if (Toutiaoapi_Com_Regex.Match(jumpToUrl).Success || Toutiao_Com_Regex.Match(jumpToUrl).Success)
        {
            return Task.FromResult(jumpToUrl);
        }

        return Task.FromResult("");
    }
}