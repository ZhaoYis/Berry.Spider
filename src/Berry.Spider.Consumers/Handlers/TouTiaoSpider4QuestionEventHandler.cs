using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：问答
/// </summary>
public class TouTiaoSpider4QuestionEventHandler : ICapSubscribe
{
    private TouTiaoSpider4QuestionProvider Provider { get; }

    public TouTiaoSpider4QuestionEventHandler(TouTiaoSpider4QuestionProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4QuestionPushEto.RoutingKeyString, Group = TouTiaoSpider4QuestionPushEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4QuestionPushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    [CapSubscribe(TouTiaoSpider4QuestionPullEto.RoutingKeyString, Group = TouTiaoSpider4QuestionPullEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4QuestionPullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}