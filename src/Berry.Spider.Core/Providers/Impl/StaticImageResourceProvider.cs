using Berry.Spider.Contracts;
using Microsoft.Extensions.Options;
using Volo.Abp.Guids;

namespace Berry.Spider.Core;

/// <summary>
/// 静态图片资源
/// </summary>
public class StaticImageResourceProvider : IImageResourceProvider
{
    public IOptionsSnapshot<ImageResourceOptions> Options { get; }
    public IGuidGenerator GuidGenerator { get; }

    public StaticImageResourceProvider(IOptionsSnapshot<ImageResourceOptions> options, IGuidGenerator generator)
    {
        this.Options = options;
        this.GuidGenerator = generator;
    }

    public string TryGet()
    {
        Random random = new Random(this.GuidGenerator.Create().GetHashCode());
        int next = random.Next(this.Options.Value.MinId, this.Options.Value.MaxId);

        string url = string.Format(this.Options.Value.TemplateUrl, next);

        return url;
    }
}