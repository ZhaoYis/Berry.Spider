using System.Threading.Tasks;
using Berry.Spider.TouTiao;

namespace Berry.Spider.Consumers;

public interface ITouTiaoSpider4HighQualityQuestionEventHandler
{
    Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionPushEto eventData);

    Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionPullEto eventData);
}