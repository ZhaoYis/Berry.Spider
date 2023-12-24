using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：资讯
/// </summary>
public class TouTiaoSpider4InformationEventHandler : ITouTiaoSpider4InformationEventHandler, ICapSubscribe
{
    private TouTiaoSpider4InformationProvider Provider { get; }

    public TouTiaoSpider4InformationEventHandler(TouTiaoSpider4InformationProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4InformationPushEto.RoutingKeyString, Group = TouTiaoSpider4InformationPushEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4InformationPushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    [CapSubscribe(TouTiaoSpider4InformationPullEto.RoutingKeyString, Group = TouTiaoSpider4InformationPullEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4InformationPullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}