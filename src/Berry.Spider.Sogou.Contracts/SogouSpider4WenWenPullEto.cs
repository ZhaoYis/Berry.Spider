using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：问问
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.Sogou_WenWen)]
public class SogouSpider4WenWenPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Sogou.WenWen.Pull";
    public const string QueueNameString = "Berry.Sogou.WenWen.Pull";

    public SogouSpider4WenWenPullEto() : base(SpiderSourceFrom.Sogou_WenWen)
    {
    }

    public SogouSpider4WenWenPullEto(SpiderSourceFrom from, string keyword, string title,
        List<ChildPageDataItem> items, string? traceCode, string identityId) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.Title = title;
        this.Items = items;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}