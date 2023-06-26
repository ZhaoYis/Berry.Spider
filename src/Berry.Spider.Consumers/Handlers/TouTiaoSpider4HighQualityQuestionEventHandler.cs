using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：优质_问答
/// </summary>
public class TouTiaoSpider4HighQualityQuestionEventHandler : ICapSubscribe
{
    private TouTiaoSpider4HighQualityQuestionProvider Provider { get; }

    public TouTiaoSpider4HighQualityQuestionEventHandler(TouTiaoSpider4HighQualityQuestionProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4HighQualityQuestionPushEto.RoutingKeyString, Group = TouTiaoSpider4HighQualityQuestionPushEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionPushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    [CapSubscribe(TouTiaoSpider4HighQualityQuestionPullEto.RoutingKeyString, Group = TouTiaoSpider4HighQualityQuestionPullEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionPullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}