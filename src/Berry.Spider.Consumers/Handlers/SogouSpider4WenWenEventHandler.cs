using Berry.Spider.Sogou;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 搜狗：问问
/// </summary>
public class SogouSpider4WenWenEventHandler : ICapSubscribe
{
    private SogouSpider4WenWenProvider Provider { get; }

    public SogouSpider4WenWenEventHandler(SogouSpider4WenWenProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(SogouSpider4WenWenPushEto.RoutingKeyString, Group = SogouSpider4WenWenPushEto.QueueNameString)]
    public async Task HandleEventAsync(SogouSpider4WenWenPushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    [CapSubscribe(SogouSpider4WenWenPullEto.RoutingKeyString, Group = SogouSpider4WenWenPullEto.QueueNameString)]
    public async Task HandleEventAsync(SogouSpider4WenWenPullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}