using Berry.Spider.Abstractions;
using Berry.Spider.Baidu;
using Berry.Spider.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider;

/// <summary>
/// 百度爬虫服务
/// </summary>
[Route("api/services/spider/baidu")]
public class BaiduSpiderController : SpiderControllerBase
{
    private IServiceProvider Provider { get; }

    public BaiduSpiderController(IServiceProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 百度：相关推荐
    /// </summary>
    [HttpPost, Route("push-related-search")]
    public Task PushAsync([FromBody] BaiduSpider4RelatedSearchPushEto push,
        [FromServices] BaiduSpider4RelatedSearchProvider provider)
    {
        return provider.PushAsync(push);
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file"), DisableRequestSizeLimit]
    public Task PushAsync(BaiduSpiderPushFromFile push)
    {
        FileHelper fileHelper = new FileHelper(push.File, row =>
        {
            if (push.SourceFrom == SpiderSourceFrom.Baidu_Related_Search)
            {
                BaiduSpider4RelatedSearchPushEto eto = new BaiduSpider4RelatedSearchPushEto
                {
                    SourceFrom = push.SourceFrom,
                    Keyword = row
                };

                ISpiderProvider provider = this.Provider.GetRequiredService<BaiduSpider4RelatedSearchProvider>();
                return provider.PushAsync(eto);
            }

            throw new NotImplementedException("未实现的爬虫来源");
        });
        return fileHelper.InvokeAsync();
    }
}