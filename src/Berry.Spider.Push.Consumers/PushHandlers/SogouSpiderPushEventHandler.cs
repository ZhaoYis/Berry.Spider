using Berry.Spider.Sogou;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 消费搜狗源数据推送消息
/// </summary>
public class SogouSpiderPushEventHandler : IDistributedEventHandler<SogouSpiderPushEto>, ITransientDependency
{
    private ISogouSpiderAppService BaiduSpiderAppService { get; }

    public SogouSpiderPushEventHandler(ISogouSpiderAppService service)
    {
        this.BaiduSpiderAppService = service;
    }

    public async Task HandleEventAsync(SogouSpiderPushEto eventData)
    {
        await this.BaiduSpiderAppService.ExecuteAsync(new SogouSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }
}