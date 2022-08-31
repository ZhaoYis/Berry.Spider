﻿using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关推荐
/// </summary>
[Spider(SpiderSourceFrom.Sogou_Related_Search)]
public class SogouSpider4RelatedSearchProvider : ISpiderProvider
{
    private ILogger<SogouSpider4RelatedSearchProvider> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }
    private ISpiderTitleContentRepository SpiderRepository { get; }

    //private string HomePage => "https://sogou.com/web?query={0}";
    private string HomePage => "https://sogou.com/web?query={0}&_asf=www.sogou.com&_ast=&w=01019900&p=40040100&ie=utf8&from=index-nologin&s_from=index&sut=383&sst0=1661865164915&lkt=0%2C0%2C0&sugsuv=008BC68EDED45858630E0B0ED06D0678&sugtime=1661865164915";

    public SogouSpider4RelatedSearchProvider(ILogger<SogouSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IDistributedEventBus eventBus,
        ISpiderTitleContentRepository repository)
    {
        this.Logger = logger;
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<SogouRelatedSearchTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
        this.SpiderRepository = repository;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public Task PushAsync<T>(T push) where T : class, ISpiderPushEto
    {
        return this.DistributedEventBus.PublishAsync(push);
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : class, ISpiderRequest
    {
        try
        {
            bool isExisted =
                await this.SpiderRepository.MyCountAsync(c => c.Title == request.Keyword && c.Published == 0) > 0;
            if (isExisted) return;

            string targetUrl = string.Format(this.HomePage, request.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                drv =>
                {
                    try
                    {
                        // return drv.FindElement(By.CssSelector(".result-molecule"));
                        return drv.FindElement(By.Id("hint_container"));
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                },
                async root =>
                {
                    if (root == null) return;

                    var resultContent = root.FindElements(By.TagName("a"));
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    if (resultContent.Count > 0)
                    {
                        var eto = new SogouSpider4RelatedSearchPullEto { Keyword = request.Keyword, Title = request.Keyword };

                        foreach (IWebElement element in resultContent)
                        {
                            string text = element.Text;
                            string href = element.GetAttribute("href");

                            eto.Items.Add(new ChildPageDataItem
                            {
                                Title = text,
                                Href = "https://sogou.com/web" + href
                            });

                            this.Logger.LogInformation(text + "  ---> " + href);
                        }

                        if (eto.Items.Any())
                        {
                            //await this.DistributedEventBus.PublishAsync(eto);

                            //此处不做消息队列发送，直接存储到数据库
                            await this.HandleEventAsync(eto);
                            this.Logger.LogInformation("数据保存成功...");
                        }
                    }
                });
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public Task HandleEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            List<SpiderTitleContent> contents = new List<SpiderTitleContent>();
            foreach (var item in eventData.Items)
            {
                var content = new SpiderTitleContent(item.Title, item.Href, eventData.SourceFrom);
                contents.Add(content);
            }

            return this.SpiderRepository.InsertManyAsync(contents);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }

        return Task.CompletedTask;
    }
}