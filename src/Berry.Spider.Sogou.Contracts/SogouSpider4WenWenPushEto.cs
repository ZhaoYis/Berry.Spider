using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.Sogou_WenWen)]
public class SogouSpider4WenWenPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Sogou.WenWen.Push";
    public const string QueueNameString = "Berry.Sogou.WenWen.Push";

    public SogouSpider4WenWenPushEto()
    {
    }

    public SogouSpider4WenWenPushEto(SpiderSourceFrom from, string keyword, string? traceCode, string identityId)
        : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}