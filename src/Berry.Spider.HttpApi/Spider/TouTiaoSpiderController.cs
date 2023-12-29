using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider;

/// <summary>
/// 今日头条爬虫服务
/// </summary>
[Area(AppGlobalConstants.ModelName)]
[Route("api/services/spider/toutiao")]
[RemoteService(Name = AppGlobalConstants.RemoteServiceName)]
public class TouTiaoSpiderController : SpiderControllerBase, ITouTiaoSpiderAppService
{
    private IServiceProvider Provider { get; }

    public TouTiaoSpiderController(IServiceProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, DisableRequestSizeLimit, Route("push-from-file")]
    public Task PushAsync([FromForm] PushFromFileBasicDto push)
    {
        object o = this.Provider.GetImplService(push.SourceFrom);
        if (o is ISpiderProvider provider)
        {
            FileHelper fileHelper = new FileHelper(push.File,
                row => provider.PushAsync(new SpiderPushToQueueDto(row, push.SourceFrom, push.TraceCode)));
            return fileHelper.InvokeAsync();
        }
        else
        {
            throw new NotImplementedException("未实现的爬虫来源");
        }
    }
}