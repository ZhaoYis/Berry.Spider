using System.Threading.Tasks;
using Berry.Spider.TouTiao;

namespace Berry.Spider.Consumers;

public interface ITouTiaoSpider4InformationEventHandler
{
    Task HandleEventAsync(TouTiaoSpider4InformationPushEto eventData);

    Task HandleEventAsync(TouTiaoSpider4InformationPullEto eventData);
}