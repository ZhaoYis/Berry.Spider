using System.Threading.Tasks;
using Berry.Spider.TouTiao;

namespace Berry.Spider.Consumers;

public interface ITouTiaoSpider4ArticleEventHandler
{
    Task HandleEventAsync(TouTiaoSpider4ArticlePushEto eventData);

    Task HandleEventAsync(TouTiaoSpider4ArticlePullEto eventData);
}