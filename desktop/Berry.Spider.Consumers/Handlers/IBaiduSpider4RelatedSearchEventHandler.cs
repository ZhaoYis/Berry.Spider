using System.Threading.Tasks;
using Berry.Spider.Baidu;

namespace Berry.Spider.Consumers;

public interface IBaiduSpider4RelatedSearchEventHandler
{
    Task HandleEventAsync(BaiduSpider4RelatedSearchPushEto eventData);

    Task HandleEventAsync(BaiduSpider4RelatedSearchPullEto eventData);
}