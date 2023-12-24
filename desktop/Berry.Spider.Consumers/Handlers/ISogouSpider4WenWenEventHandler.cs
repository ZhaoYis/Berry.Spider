using System.Threading.Tasks;
using Berry.Spider.Sogou;

namespace Berry.Spider.Consumers;

public interface ISogouSpider4WenWenEventHandler
{
    Task HandleEventAsync(SogouSpider4WenWenPushEto eventData);

    Task HandleEventAsync(SogouSpider4WenWenPullEto eventData);
}