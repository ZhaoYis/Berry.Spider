using Berry.Spider.Core;
using Berry.Spider.Domain.TouTiao;
using Berry.Spider.TouTiao;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 头条：问答
/// </summary>
public class TouTiaoSpider4QuestionEventHandler : IDistributedEventHandler<TouTiaoSpider4QuestionEto>,
    ITransientDependency
{
    private ILogger<TouTiaoSpider4QuestionEventHandler> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private ITouTiaoSpiderRepository TiaoSpiderRepository { get; }

    public TouTiaoSpider4QuestionEventHandler(ILogger<TouTiaoSpider4QuestionEventHandler> logger,
        IWebElementLoadProvider provider, ITextAnalysisProvider analysisProvider, ITouTiaoSpiderRepository repository)
    {
        this.Logger = logger;
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = analysisProvider;
        this.TiaoSpiderRepository = repository;
    }

    public async Task HandleEventAsync(TouTiaoSpider4QuestionEto eventData)
    {
        try
        {
            List<string> contentItems = new List<string>();

            if (eventData.Items.Any())
            {
                foreach (var item in eventData.Items)
                {
                    await this.WebElementLoadProvider.InvokeAsync(
                        item.Href,
                        drv =>
                        {
                            try
                            {
                                return drv.FindElement(By.ClassName("s-container"));
                            }
                            catch (Exception e)
                            {
                                return null;
                            }
                        },
                        async root =>
                        {
                            if (root == null) return;

                            var resultContent = root.FindElements(By.ClassName("list"));
                            if (resultContent.Count > 0)
                            {
                                foreach (IWebElement element in resultContent)
                                {
                                    var answerList = element
                                        .FindElements(By.TagName("div"))
                                        .Where(c => c.GetAttribute("class").StartsWith("answer_layout_wrapper_"))
                                        .ToList();
                                    if (answerList.Any())
                                    {
                                        foreach (IWebElement answer in answerList)
                                        {
                                            if (answer != null && !string.IsNullOrWhiteSpace(answer.Text))
                                            {
                                                //解析内容
                                                var list = await this.TextAnalysisProvider.InvokeAsync(answer.Text);
                                                if (list.Count > 0)
                                                {
                                                    contentItems.AddRange(list);
                                                    this.Logger.LogInformation("总共获取到记录：" + list.Count);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    );
                }

                //去重
                contentItems = contentItems.Distinct().ToList();
                //打乱
                contentItems.RandomSort();

                string mainContent = contentItems.BuildMainContent();
                if (!string.IsNullOrEmpty(mainContent))
                {
                    var content = new TouTiaoSpiderContent(eventData.Title, mainContent, eventData.SourceFrom);
                    await this.TiaoSpiderRepository.InsertAsync(content);
                }
            }
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
}