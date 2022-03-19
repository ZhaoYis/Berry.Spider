using System.Linq;
using System.Threading.Tasks;
using Berry.Spider.Baidu;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 消费百度源数据推送消息
/// </summary>
public class BaiduSpiderPushEventHandler : IDistributedEventHandler<BaiduSpiderPushEto>, ITransientDependency
{
    private IBaiduSpiderAppService BaiduSpiderAppService { get; }

    public BaiduSpiderPushEventHandler(IBaiduSpiderAppService service)
    {
        this.BaiduSpiderAppService = service;
    }

    public async Task HandleEventAsync(BaiduSpiderPushEto eventData)
    {
        await this.BaiduSpiderAppService.ExecuteAsync(new BaiduSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }
}