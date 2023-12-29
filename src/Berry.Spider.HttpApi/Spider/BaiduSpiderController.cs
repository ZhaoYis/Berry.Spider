using Berry.Spider.Abstractions;
using Berry.Spider.Baidu;
using Berry.Spider.Core;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider;

/// <summary>
/// 百度爬虫服务
/// </summary>
[Area(AppGlobalConstants.ModelName)]
[Route("api/services/spider/baidu")]
[RemoteService(Name = AppGlobalConstants.RemoteServiceName)]
public class BaiduSpiderController : SpiderControllerBase, IBaiduSpiderAppService
{
    private IServiceProvider Provider { get; }

    public BaiduSpiderController(IServiceProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file"), DisableRequestSizeLimit]
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