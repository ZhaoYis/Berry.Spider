﻿using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.TouTiao.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：资讯
/// </summary>
[Dependency(ServiceLifetime.Transient), ExposeServices(typeof(ITouTiaoSpiderProvider))]
public class TouTiaoSpider4InformationProvider : ITouTiaoSpiderProvider
{
    private ILogger<TouTiaoSpider4InformationProvider> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc";

    public TouTiaoSpider4InformationProvider(ILogger<TouTiaoSpider4InformationProvider> logger,
        IWebElementLoadProvider provider,
        IDistributedEventBus eventBus)
    {
        this.Logger = logger;
        this.WebElementLoadProvider = provider;
        this.DistributedEventBus = eventBus;
    }

    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        string targetUrl = string.Format(this.HomePage, request.Keyword);
        await this.WebElementLoadProvider.InvokeAsync(
            targetUrl,
            drv => drv.FindElement(By.ClassName("s-result-list")),
            async root =>
            {
                var resultContent = root.FindElements(By.ClassName("result-content"));
                this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                if (resultContent.Count > 0)
                {
                    var eto = new TouTiaoSpider4QuestionEto() {Keyword = request.Keyword, Title = request.Keyword};

                    foreach (IWebElement element in resultContent)
                    {
                        try
                        {
                            var a = element.FindElement(By.TagName("a"));
                            if (a != null)
                            {
                                string text = a.Text;
                                string href = a.GetAttribute("href");

                                eto.Items.Add(new SpiderChildPageDataItem
                                {
                                    Title = text,
                                    Href = href
                                });

                                this.Logger.LogInformation(text + "  ---> " + href);
                            }
                        }
                        catch (WebDriverException exception)
                        {
                            this.Logger.LogException(exception);
                        }
                        catch (Exception exception)
                        {
                            this.Logger.LogException(exception);
                        }
                    }

                    if (eto.Items.Any())
                    {
                        await this.DistributedEventBus.PublishAsync(eto);
                        this.Logger.LogInformation("事件发布成功，等待消费...");
                    }
                }
            });
    }
}