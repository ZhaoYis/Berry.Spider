using Berry.Spider.Abstractions;
using Berry.Spider.Core;
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
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, DisableRequestSizeLimit, Route("push-from-file")]
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