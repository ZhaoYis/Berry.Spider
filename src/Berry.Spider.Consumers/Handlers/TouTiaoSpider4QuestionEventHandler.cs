using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Berry.Spider.Domain.TouTiao;
using Berry.Spider.TouTiao.Contracts;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 头条：问答
/// </summary>
public class TouTiaoSpider4QuestionEventHandler : IDistributedEventHandler<TouTiaoSpider4QuestionEto>,
    ITransientDependency
{
    private ITouTiaoSpiderRepository TiaoSpiderRepository { get; }

    public TouTiaoSpider4QuestionEventHandler(ITouTiaoSpiderRepository repository)
    {
        this.TiaoSpiderRepository = repository;
    }

    public async Task HandleEventAsync(TouTiaoSpider4QuestionEto eventData)
    {
        List<TouTiaoSpiderContent> contents = new List<TouTiaoSpiderContent>();

        if (eventData.Items.Any())
        {
            eventData.Items.ForEach(c =>
            {
                //TODO:根据实际href获取具体content信息
                contents.Add(new TouTiaoSpiderContent(c.Title, c.Href, eventData.SourceFrom, c.Href));
            });

            await this.TiaoSpiderRepository.InsertManyAsync(contents);
        }
    }
}