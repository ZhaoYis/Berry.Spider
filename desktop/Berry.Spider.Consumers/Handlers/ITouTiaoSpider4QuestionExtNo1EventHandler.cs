using System.Threading.Tasks;
using Berry.Spider.TouTiao;

namespace Berry.Spider.Consumers;

public interface ITouTiaoSpider4QuestionExtNo1EventHandler
{
    Task HandleEventAsync(TouTiaoSpider4QuestionExtNo1PushEto eventData);

    Task HandleEventAsync(TouTiaoSpider4QuestionExtNo1PullEto eventData);
}