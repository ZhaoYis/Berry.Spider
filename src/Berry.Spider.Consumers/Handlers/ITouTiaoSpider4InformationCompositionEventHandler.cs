using System.Threading.Tasks;
using Berry.Spider.TouTiao;

namespace Berry.Spider.Consumers;

public interface ITouTiaoSpider4InformationCompositionEventHandler
{
    Task HandleEventAsync(TouTiaoSpider4InformationCompositionPushEto eventData);

    Task HandleEventAsync(TouTiaoSpider4InformationCompositionPullEto eventData);
}