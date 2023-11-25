using System.Threading.Tasks;
using Berry.Spider.Sogou;

namespace Berry.Spider.Consumers;

public interface ISogouSpider4RelatedSearchEventHandler
{
    Task HandleEventAsync(SogouSpider4RelatedSearchPushEto eventData);

    Task HandleEventAsync(SogouSpider4RelatedSearchPullEto eventData);
}