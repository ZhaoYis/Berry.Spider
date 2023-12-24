using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：问答
/// </summary>
public class TouTiaoSpider4QuestionExtNo1EventHandler : ITouTiaoSpider4QuestionExtNo1EventHandler, ICapSubscribe
{
    private TouTiaoSpider4QuestionProvider Provider { get; }

    public TouTiaoSpider4QuestionExtNo1EventHandler(TouTiaoSpider4QuestionProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4QuestionExtNo1PushEto.RoutingKeyString, Group = TouTiaoSpider4QuestionExtNo1PushEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4QuestionExtNo1PushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    [CapSubscribe(TouTiaoSpider4QuestionExtNo1PullEto.RoutingKeyString, Group = TouTiaoSpider4QuestionExtNo1PullEto.QueueNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4QuestionExtNo1PullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}