using Berry.Spider.Contracts;
using Microsoft.Extensions.Options;
using Volo.Abp.Guids;

namespace Berry.Spider.Core;

/// <summary>
/// 静态图片资源
/// </summary>
public class StaticImageResourceProvider : IImageResourceProvider
{
    public ImageResourceOptions Options { get; }
    public IGuidGenerator GuidGenerator { get; }

    public StaticImageResourceProvider(IOptionsSnapshot<ImageResourceOptions> options, IGuidGenerator generator)
    {
        this.Options = options.Value;
        this.GuidGenerator = generator;
    }

    public string TryGet()
    {
        Random random = new Random(this.GuidGenerator.Create().GetHashCode());
        int next = random.Next(this.Options.MinId, this.Options.MaxId);

        string url = string.Format(this.Options.TemplateUrl, next);

        return url;
    }
}