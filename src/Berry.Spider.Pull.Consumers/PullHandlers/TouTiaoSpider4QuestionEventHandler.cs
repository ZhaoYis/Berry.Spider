using Berry.Spider.TouTiao;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 头条：问答
/// </summary>
public class TouTiaoSpider4QuestionEventHandler : IDistributedEventHandler<TouTiaoSpider4QuestionEto>,
    ITransientDependency
{
    private ILogger<TouTiaoSpider4QuestionEventHandler> Logger { get; }
    private ITouTiaoSpiderAppService TouTiaoSpiderAppService { get; }

    public TouTiaoSpider4QuestionEventHandler(ILogger<TouTiaoSpider4QuestionEventHandler> logger,
        ITouTiaoSpiderAppService service)
    {
        this.Logger = logger;
        this.TouTiaoSpiderAppService = service;
    }

    public async Task HandleEventAsync(TouTiaoSpider4QuestionEto eventData)
    {
        try
        {
            await this.TouTiaoSpiderAppService.HandleEventAsync<TouTiaoSpider4QuestionEto>(eventData);
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