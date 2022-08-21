using Berry.Spider.Sogou;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 搜狗：相关推荐
/// </summary>
public class SogouSpider4RelatedSearchEventHandler : IDistributedEventHandler<SogouSpider4RelatedSearchEto>,
    ITransientDependency
{
    private ILogger<SogouSpider4RelatedSearchEventHandler> Logger { get; }
    private ISogouSpiderAppService SogouSpiderAppService { get; }

    public SogouSpider4RelatedSearchEventHandler(ILogger<SogouSpider4RelatedSearchEventHandler> logger,
        ISogouSpiderAppService service)
    {
        this.Logger = logger;
        this.SogouSpiderAppService = service;
    }

    public async Task HandleEventAsync(SogouSpider4RelatedSearchEto eventData)
    {
        try
        {
            await this.SogouSpiderAppService.HandleEventAsync<SogouSpider4RelatedSearchEto>(eventData);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }
    }
}