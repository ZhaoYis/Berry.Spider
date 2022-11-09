using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 今日头条爬虫服务
/// </summary>
[Route("api/services/spider/toutiao")]
public class TouTiaoSpiderController : SpiderControllerBase
{
    private IServiceProvider Provider { get; }

    public TouTiaoSpiderController(IServiceProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 头条：问答
    /// </summary>
    [HttpPost, Route("push-question")]
    public Task PushAsync([FromBody] PushKeywordBasicDto push,
        [FromServices] TouTiaoSpider4QuestionProvider provider)
    {
        return provider.PushAsync(push.Keyword);
    }

    /// <summary>
    /// 头条：优质_问答
    /// </summary>
    [HttpPost, Route("push-highQuality-question")]
    public Task PushAsync([FromBody] PushKeywordBasicDto push,
        [FromServices] TouTiaoSpider4HighQualityQuestionProvider provider)
    {
        return provider.PushAsync(push.Keyword);
    }

    /// <summary>
    /// 头条：资讯
    /// </summary>
    [HttpPost, Route("push-information")]
    public Task PushAsync([FromBody] PushKeywordBasicDto push,
        [FromServices] TouTiaoSpider4InformationProvider provider)
    {
        return provider.PushAsync(push.Keyword);
    }

    /// <summary>
    /// 头条：头条_资讯_作文板块
    /// </summary>
    [HttpPost, Route("push-information-composition")]
    public Task PushAsync([FromBody] PushKeywordBasicDto push,
        [FromServices] TouTiaoSpider4InformationCompositionProvider provider)
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