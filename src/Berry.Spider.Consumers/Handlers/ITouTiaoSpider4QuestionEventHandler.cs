using System.Threading.Tasks;
using Berry.Spider.TouTiao;

namespace Berry.Spider.Consumers;

public interface ITouTiaoSpider4QuestionEventHandler
{
    Task HandleEventAsync(TouTiaoSpider4QuestionPushEto eventData);

    Task HandleEventAsync(TouTiaoSpider4QuestionPullEto eventData);
}