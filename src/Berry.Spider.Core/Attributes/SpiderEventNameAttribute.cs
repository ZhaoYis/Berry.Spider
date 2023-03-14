using Volo.Abp.EventBus;

namespace Berry.Spider.Core;

/// <summary>
/// 自定义事件特性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SpiderEventNameAttribute : EventNameAttribute
{
    public SpiderEventNameAttribute(string name, SpiderSourceFrom from) : base(name)
    {
        this.SourceFrom = from;
    }

    public SpiderSourceFrom SourceFrom { get; set; }
}