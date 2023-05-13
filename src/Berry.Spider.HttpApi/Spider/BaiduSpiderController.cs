using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Microsoft.AspNetCore.Mvc;

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
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file"), DisableRequestSizeLimit]
    public Task PushAsync(PushFromFileBasicDto push)
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