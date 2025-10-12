using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：文章
/// </summary>
public sealed class TouTiaoSpider4ArticleEventHandler(TouTiaoSpider4ArticleProvider provider) : ITouTiaoSpider4ArticleEventHandler, ICapSubscribe
{
    private TouTiaoSpider4ArticleProvider Provider { get; } = provider;

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4ArticlePushEto.RoutingKeyString, Group = TouTiaoSpider4ArticlePushEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4ArticlePushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    [CapSubscribe(TouTiaoSpider4ArticlePullEto.RoutingKeyString, Group = TouTiaoSpider4ArticlePullEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4ArticlePullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}