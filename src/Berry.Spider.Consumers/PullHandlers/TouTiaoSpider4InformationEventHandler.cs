using Berry.Spider.TouTiao;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Berry.Spider.Domain;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 头条：资讯
/// </summary>
public class TouTiaoSpider4InformationEventHandler : IDistributedEventHandler<TouTiaoSpider4InformationEto>,
    ITransientDependency
{
    private ISpiderContentRepository SpiderRepository { get; }

    public TouTiaoSpider4InformationEventHandler(ISpiderContentRepository repository)
    {
        this.SpiderRepository = repository;
    }

    public async Task HandleEventAsync(TouTiaoSpider4InformationEto eventData)
    {
        List<SpiderContent> contents = new List<SpiderContent>();

        if (eventData.Items.Any())
        {
            foreach (var item in eventData.Items)
            {
                //TODO:获取具体内容
                contents.Add(new SpiderContent(item.Title, item.Href, eventData.SourceFrom, item.Href));
            }

            await this.SpiderRepository.InsertManyAsync(contents);
        }
    }
}