using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Berry.Spider.Domain.Shared;
using Berry.Spider.Domain.TouTiao;
using Berry.Spider.TouTiao;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 头条消息消费者
/// </summary>
public class TouTiaoSpiderEventHandler : IDistributedEventHandler<TouTiaoSpiderEto>, ITransientDependency
{
    private ITouTiaoSpiderRepository TiaoSpiderRepository { get; }

    public TouTiaoSpiderEventHandler(ITouTiaoSpiderRepository repository)
    {
        this.TiaoSpiderRepository = repository;
    }

    public async Task HandleEventAsync(TouTiaoSpiderEto eventData)
    {
        List<TouTiaoSpiderContent> contents = new List<TouTiaoSpiderContent>();

        if (eventData.Items.Any())
        {
            eventData.Items.ForEach(c =>
            {
                contents.Add(new TouTiaoSpiderContent(c.Title, c.Href, SpiderSourceFrom.TouTiao, c.Href));
            });

            await this.TiaoSpiderRepository.InsertManyAsync(contents);
        }
    }
}