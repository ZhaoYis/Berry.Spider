using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：优质_问答_扩展_01
/// </summary>
public class TouTiaoSpider4HighQualityQuestionExtNo1EventHandler : ITouTiaoSpider4HighQualityQuestionExtNo1EventHandler, ICapSubscribe
{
    private TouTiaoSpider4HighQualityQuestionProvider Provider { get; }

    public TouTiaoSpider4HighQualityQuestionExtNo1EventHandler(TouTiaoSpider4HighQualityQuestionProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4HighQualityQuestionExtNo1PushEto.RoutingKeyString, Group = TouTiaoSpider4HighQualityQuestionExtNo1PushEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionExtNo1PushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    [CapSubscribe(TouTiaoSpider4HighQualityQuestionExtNo1PullEto.RoutingKeyString, Group = TouTiaoSpider4HighQualityQuestionExtNo1PullEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionExtNo1PullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}