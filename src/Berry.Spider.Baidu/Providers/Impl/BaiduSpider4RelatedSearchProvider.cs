using Berry.Spider.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关推荐
/// </summary>
public class BaiduSpider4RelatedSearchProvider : IBaiduSpiderProvider
{
    private ILogger<BaiduSpider4RelatedSearchProvider> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    private string HomePage => "";

    public BaiduSpider4RelatedSearchProvider(ILogger<BaiduSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IDistributedEventBus eventBus)
    {
        this.Logger = logger;
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<BaiduRelatedSearchTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        //TODO:待实现

        //采集百度相关搜索并拿出标题
        var eto = new BaiduSpider4RelatedSearchEto {Keyword = request.Keyword, Title = request.Keyword};
        eto.Items.Add(new ChildPageDataItem {Title = "", Href = ""});
        if (eto.Items.Any())
        {
            await this.DistributedEventBus.PublishAsync(eto);
            this.Logger.LogInformation("事件发布成功，等待消费...");
        }
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public async Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto
    {
        //TODO:入库操作
        await Task.CompletedTask;
    }
}