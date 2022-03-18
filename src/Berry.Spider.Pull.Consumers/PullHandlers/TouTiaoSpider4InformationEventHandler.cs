using Berry.Spider.TouTiao;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 头条：资讯
/// </summary>
public class TouTiaoSpider4InformationEventHandler : IDistributedEventHandler<TouTiaoSpider4InformationEto>,
    ITransientDependency
{
    private ILogger<TouTiaoSpider4InformationEventHandler> Logger { get; }
    private ITouTiaoSpiderAppService TouTiaoSpiderAppService { get; }

    public TouTiaoSpider4InformationEventHandler(ILogger<TouTiaoSpider4InformationEventHandler> logger,
        ITouTiaoSpiderAppService service)
    {
        this.Logger = logger;
        this.TouTiaoSpiderAppService = service;
    }

    public async Task HandleEventAsync(TouTiaoSpider4InformationEto eventData)
    {
        try
        {
            await this.TouTiaoSpiderAppService.HandleEventAsync<TouTiaoSpider4InformationEto>(eventData);
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