using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain.Shared;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;
using Volo.Abp.TextTemplating;

namespace Berry.Spider.Domain;

/// <summary>
/// 爬虫领域服务
/// </summary>
public class SpiderDomainService : DomainService
{
    private IImageResourceProvider ImageResourceProvider { get; }
    private ITemplateRenderer TemplateRenderer { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    public SpiderDomainService(
        IImageResourceProvider imageResourceProvider,
        ITemplateRenderer templateRenderer,
        IOptionsSnapshot<SpiderOptions> options)
    {
        ImageResourceProvider = imageResourceProvider;
        TemplateRenderer = templateRenderer;
        Options = options;
    }

    /// <summary>
    /// 统一构建落库实体内容
    /// </summary>
    /// <returns></returns>
    public async Task<SpiderContent?> BuildContentAsync(string originalTitle, SpiderSourceFrom sourceFrom, List<string> contentItems)
    {
        if (contentItems.Count >= this.Options.Value.MinRecords)
        {
            //打乱
            contentItems.RandomSort();

            string mainContent;
            if (this.Options.Value.IsInsertImage)
            {
                if (this.Options.Value.IsRandomInsertImage)
                {
                    mainContent = this.Clock.Now.Hour % 2 == 0
                        ? contentItems.BuildMainContent(this.ImageResourceProvider)
                        : contentItems.BuildMainContent();
                }
                else
                {
                    mainContent = contentItems.BuildMainContent(this.ImageResourceProvider);
                }
            }
            else
            {
                mainContent = contentItems.BuildMainContent();
            }

            if (!string.IsNullOrEmpty(mainContent))
            {
                //随机获取一个模版名称
                string[] names = Enum.GetNames<TitleTemplateNames>();
                int index = new Random(this.GuidGenerator.Create().GetHashCode()).Next(0, names.Length - 1);
                string titleTemplateName = names[index];

                //重写Title
                string title = await this.TemplateRenderer.RenderAsync(titleTemplateName, new
                {
                    OriginalTitle = originalTitle,
                    Total = contentItems.Count,
                });

                //组装数据
                var content = new SpiderContent(title, mainContent, sourceFrom);
                return content;
            }
        }

        return default;
    }
}