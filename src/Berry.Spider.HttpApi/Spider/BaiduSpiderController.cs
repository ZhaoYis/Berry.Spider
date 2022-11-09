﻿using Berry.Spider.Abstractions;
using Berry.Spider.Baidu;
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
    /// 百度：相关推荐
    /// </summary>
    [HttpPost, Route("push-related-search")]
    public Task PushAsync([FromBody] PushKeywordBasicDto push,
        [FromServices] BaiduSpider4RelatedSearchProvider provider)
    {
        return provider.PushAsync(push.Keyword);
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file"), DisableRequestSizeLimit]
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