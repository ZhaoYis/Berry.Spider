using Berry.Spider.TouTiao;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 消费头条爬虫消息
/// </summary>
public class TouTiaoSpiderPushEventHandler : IDistributedEventHandler<TouTiaoSpiderPushEto>, ITransientDependency
{
    private ITouTiaoSpiderAppService TouTiaoSpiderAppService { get; }

    public TouTiaoSpiderPushEventHandler(ITouTiaoSpiderAppService service)
    {
        this.TouTiaoSpiderAppService = service;
    }

    public async Task HandleEventAsync(TouTiaoSpiderPushEto eventData)
    {
        if (eventData.Keywords.Any())
        {
            foreach (string keyword in eventData.Keywords)
            {
                await this.TouTiaoSpiderAppService.PublishAsync(new TouTiaoSpiderRequest
                {
                    SourceFrom = eventData.SourceFrom,
                    Keyword = keyword
                });
            }
        }
    }
}