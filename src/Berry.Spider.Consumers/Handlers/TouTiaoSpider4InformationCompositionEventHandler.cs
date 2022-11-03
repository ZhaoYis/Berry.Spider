using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：资讯_作文
/// </summary>
public class TouTiaoSpider4InformationCompositionEventHandler : ICapSubscribe
{
    private TouTiaoSpider4InformationCompositionProvider Provider { get; }

    public TouTiaoSpider4InformationCompositionEventHandler(TouTiaoSpider4InformationCompositionProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4InformationCompositionPushEto.RoutingKeyString, Group = TouTiaoSpider4InformationCompositionPushEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4InformationCompositionPushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4InformationCompositionPullEto.RoutingKeyString, Group = TouTiaoSpider4InformationCompositionPullEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4InformationCompositionPullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}