using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
    public Task PushAsync([FromBody] TouTiaoSpider4QuestionPushEto push,
        [FromServices] TouTiaoSpider4QuestionProvider provider)
    {
        return provider.PushAsync(push);
    }

    /// <summary>
    /// 头条：资讯
    /// </summary>
    [HttpPost, Route("push-information")]
    public Task PushAsync([FromBody] TouTiaoSpider4InformationPushEto push,
        [FromServices] TouTiaoSpider4InformationProvider provider)
    {
        return provider.PushAsync(push);
    }

    /// <summary>
    /// 头条：头条_资讯_作文板块
    /// </summary>
    [HttpPost, Route("push-information-composition")]
    public Task PushAsync([FromBody] TouTiaoSpider4InformationPushEto push,
        [FromServices] TouTiaoSpider4InformationCompositionProvider provider)
    {
        return provider.PushAsync(push);
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file"), DisableRequestSizeLimit]
    public Task PushAsync(TouTiaoSpiderPushFromFile push)
    {
        FileHelper fileHelper = new FileHelper(push.File, row =>
        {
            //TODO:通过反射的方式获取到对应的provider，特性名称SpiderAttribute
            if (push.SourceFrom == SpiderSourceFrom.TouTiao_Question)
            {
                TouTiaoSpider4QuestionPushEto eto = new TouTiaoSpider4QuestionPushEto
                {
                    SourceFrom = push.SourceFrom,
                    Keyword = row
                };

                ISpiderProvider provider = this.Provider.GetRequiredService<TouTiaoSpider4QuestionProvider>();
                return provider.PushAsync(eto);
            }
            else if (push.SourceFrom == SpiderSourceFrom.TouTiao_Information)
            {
                TouTiaoSpider4InformationPushEto eto = new TouTiaoSpider4InformationPushEto
                {
                    SourceFrom = push.SourceFrom,
                    Keyword = row
                };

                ISpiderProvider provider = this.Provider.GetRequiredService<TouTiaoSpider4InformationProvider>();
                return provider.PushAsync(eto);
            }
            else if (push.SourceFrom == SpiderSourceFrom.TouTiao_Information_Composition)
            {
                TouTiaoSpider4InformationCompositionPushEto eto = new TouTiaoSpider4InformationCompositionPushEto
                {
                    SourceFrom = push.SourceFrom,
                    Keyword = row
                };

                ISpiderProvider provider = this.Provider.GetRequiredService<TouTiaoSpider4InformationCompositionProvider>();
                return provider.PushAsync(eto);
            }

            throw new NotImplementedException("未实现的爬虫来源");
        });
        return fileHelper.InvokeAsync();
    }
}