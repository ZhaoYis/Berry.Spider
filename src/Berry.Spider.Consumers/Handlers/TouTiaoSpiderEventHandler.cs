using System.Threading.Tasks;
using Berry.Spider.TouTiao;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 头条消息消费者
/// </summary>
public class TouTiaoSpiderEventHandler : IDistributedEventHandler<TouTiaoSpiderEto>, ITransientDependency
{
    public Task HandleEventAsync(TouTiaoSpiderEto eventData)
    {
        return Task.CompletedTask;
    }
}