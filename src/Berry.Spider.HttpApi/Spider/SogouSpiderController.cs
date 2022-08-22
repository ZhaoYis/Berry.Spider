using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Sogou;
using Berry.Spider.Sogou.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider;

/// <summary>
/// 搜狗爬虫服务
/// </summary>
[Route("api/services/spider/sogou")]
public class SogouSpiderController : SpiderControllerBase
{
    private IServiceProvider Provider { get; }

    public SogouSpiderController(IServiceProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 搜狗：相关推荐
    /// </summary>
    [HttpPost, Route("push-related-search")]
    public Task PushAsync([FromBody] SogouSpider4RelatedSearchPushEto push,
        [FromServices] SogouSpider4RelatedSearchProvider provider)
    {
        return provider.PushAsync(push);
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file"), DisableRequestSizeLimit]
    public Task PushAsync(SogouSpiderPushFromFile push)
    {
        FileHelper fileHelper = new FileHelper(push.File, row =>
        {
            if (push.SourceFrom == SpiderSourceFrom.Sogou_Related_Search)
            {
                SogouSpider4RelatedSearchPushEto eto = new SogouSpider4RelatedSearchPushEto
                {
                    SourceFrom = push.SourceFrom,
                    Keyword = row
                };

                ISpiderProvider provider = this.Provider.GetRequiredService<SogouSpider4RelatedSearchProvider>();
                return provider.PushAsync(eto);
            }

            throw new NotImplementedException("未实现的爬虫来源");
        });
        return fileHelper.InvokeAsync();
    }
}