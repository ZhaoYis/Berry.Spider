using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Sogou;
using Microsoft.AspNetCore.Mvc;

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
    public Task PushAsync([FromBody] PushKeywordBasicDto push,
        [FromServices] SogouSpider4RelatedSearchProvider provider)
    {
        return provider.PushAsync(push.Keyword);
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, DisableRequestSizeLimit, Route("push-from-file")]
    public Task PushAsync(PushFromFileBasicDto push)
    {
        FileHelper fileHelper = new FileHelper(push.File, row =>
        {
            object o = this.Provider.GetImplService(push.SourceFrom);
            if (o is ISpiderProvider provider)
            {
                return provider.PushAsync(row);
            }
            else
            {
                throw new NotImplementedException("未实现的爬虫来源");
            }
        });
        return fileHelper.InvokeAsync();
    }
}