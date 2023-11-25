using System.Threading.Tasks;
using Berry.Spider.TouTiao;

namespace Berry.Spider.Consumers;

public interface ITouTiaoSpider4HighQualityQuestionExtNo1EventHandler
{
    Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionExtNo1PushEto eventData);

    Task HandleEventAsync(TouTiaoSpider4HighQualityQuestionExtNo1PullEto eventData);
}