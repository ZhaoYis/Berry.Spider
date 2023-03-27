﻿using Berry.Spider.Abstractions;
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
                return provider.PushAsync(row, push.SourceFrom);
            }
            else
            {
                throw new NotImplementedException("未实现的爬虫来源");
            }
        });
        return fileHelper.InvokeAsync();
    }
}