using System.Threading.Tasks;
using Berry.Spider.Abstractions;
using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：资讯_作文
/// </summary>
public class TouTiaoSpider4InformationCompositionEventHandler : ICapSubscribe
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4InformationCompositionPushEto.RoutingKeyString, Group = TouTiaoSpider4InformationCompositionPushEto.QueueNameString)]
    public Task HandleEventAsync(TouTiaoSpider4InformationCompositionPushEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationCompositionProvider>();

        return provider.ExecuteAsync(new TouTiaoSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4InformationCompositionPullEto.RoutingKeyString, Group = TouTiaoSpider4InformationCompositionPullEto.QueueNameString)]
    public Task HandleEventAsync(TouTiaoSpider4InformationCompositionPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationCompositionProvider>();

        return provider.HandleEventAsync(eventData);
    }
}