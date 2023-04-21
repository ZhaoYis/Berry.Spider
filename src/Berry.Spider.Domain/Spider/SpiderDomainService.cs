using System.Collections.Immutable;
using System.Text;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Segmenter;
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
    private ISegmenterProvider SegmenterProvider { get; }
    private IStringBuilderObjectPoolProvider StringBuilderObjectPoolProvider { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }
    private IOptionsSnapshot<TitleTemplateContentOptions> TitleTemplateOptions { get; }

    public SpiderDomainService(
        IImageResourceProvider imageResourceProvider,
        ITemplateRenderer templateRenderer,
        ISegmenterProvider segmenterProvider,
        IStringBuilderObjectPoolProvider stringBuilderObjectPoolProvider,
        IOptionsSnapshot<SpiderOptions> options,
        IOptionsSnapshot<TitleTemplateContentOptions> titleTemplateOptions)
    {
        ImageResourceProvider = imageResourceProvider;
        TemplateRenderer = templateRenderer;
        SegmenterProvider = segmenterProvider;
        StringBuilderObjectPoolProvider = stringBuilderObjectPoolProvider;
        Options = options;
        TitleTemplateOptions = titleTemplateOptions;
    }

    /// <summary>
    /// 统一构建落库实体内容
    /// </summary>
    /// <returns></returns>
    public async Task<SpiderContent?> BuildContentAsync(string originalTitle, SpiderSourceFrom sourceFrom,
        List<string> contentItems, List<string>? subTitleList = null)
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
                        ? contentItems.BuildMainContent(this.ImageResourceProvider, this.StringBuilderObjectPoolProvider, subTitleList)
                        : contentItems.BuildMainContent(this.StringBuilderObjectPoolProvider, originalTitle, subTitleList);
                }
                else
                {
                    mainContent = contentItems.BuildMainContent(this.ImageResourceProvider, this.StringBuilderObjectPoolProvider, subTitleList);
                }
            }
            else
            {
                mainContent = contentItems.BuildMainContent(this.StringBuilderObjectPoolProvider, originalTitle, subTitleList);
            }

            if (mainContent is { Length: > 0 })
            {
                //处理子标题
                if (this.Options.Value.SubTitleOptions.IsEnable)
                {
                    var opSubTitleList = subTitleList.Take(this.Options.Value.SubTitleOptions.MaxRecords).ToList();

                    var subTitleContent = opSubTitleList.BuildSubTitleContent(this.StringBuilderObjectPoolProvider);
                    if (subTitleContent is { Length: > 0 })
                    {
                        mainContent = mainContent.Insert(0, subTitleContent);
                    }
                }

                if (this.TitleTemplateOptions.Value.IsEnableFormatTitle)
                {
                    //随机获取一个模版名称
                    List<string> names = this.TitleTemplateOptions.Value.Templates.Select(c => c.Name).ToList();
                    if (names.Count > 0)
                    {
                        int index = new Random(this.GuidGenerator.Create().GetHashCode()).Next(0, names.Count - 1);
                        string titleTemplateName = names[index];

                        //重写Title
                        string title = await this.TemplateRenderer.RenderAsync(titleTemplateName, new
                        {
                            OriginalTitle = originalTitle,
                            Total = contentItems.Count,
                        });

                        originalTitle = title;
                    }
                }

                //组装数据
                var content = new SpiderContent(originalTitle, mainContent.ToString(), sourceFrom);

                //TODO：根据配置决定是否需要进行分词操作
                //TODO：或许可以重构成服务，其他使用的地方无需关注这些逻辑
                // //对标题进行分词操作
                // var segments = await this.SegmenterProvider.CutForSearchAsync(originalTitle);
                // if (segments is { Count: > 0 })
                // {
                //     content.Keywords = string.Join(" ", segments);
                // }

                return content;
            }
        }

        return default;
    }

    /// <summary>
    /// 统一构建落库实体内容（优质问答）
    /// </summary>
    /// <returns></returns>
    public Task<SpiderContent_HighQualityQA> BuildHighQualityContentAsync(string originalTitle,
        SpiderSourceFrom sourceFrom,
        IDictionary<string, List<string>> contentItems)
    {
        ImmutableList<string> subTitleList = ImmutableList.Create<string>();
        string mainContent = this.StringBuilderObjectPoolProvider.Invoke(mainContentBuilder =>
        {
            foreach (KeyValuePair<string, List<string>> item in contentItems)
            {
                string title = item.Key;
                List<string> answerContentItems = item.Value;

                //打乱
                answerContentItems.RandomSort();
                //取指定的记录数
                answerContentItems = answerContentItems
                    .Take(this.Options.Value.HighQualityAnswerOptions.MaxRecordCount)
                    .ToList();

                StringBuilder itemContentBuilder = new StringBuilder();
                for (int i = 0; i < answerContentItems.Count; i++)
                {
                    string itemContent = answerContentItems[i];
                    itemContentBuilder.AppendFormat("<p id='{0}'><strong>{0}</strong></p>", title);
                    itemContentBuilder.AppendFormat("<p>{0}</p>", itemContent);

                    subTitleList = subTitleList.Add(title);
                }
                mainContentBuilder.Append(itemContentBuilder);
            }
        });

        if (mainContent is { Length: > 0 })
        {
            //处理子标题
            if (this.Options.Value.SubTitleOptions.IsEnable)
            {
                var opSubTitleList = subTitleList.Take(this.Options.Value.SubTitleOptions.MaxRecords).ToList();

                var subTitleContent = opSubTitleList.BuildSubTitleContent(this.StringBuilderObjectPoolProvider);
                if (subTitleContent is { Length: > 0 })
                {
                    mainContent = mainContent.Insert(0, subTitleContent);
                }
            }
        }

        //组装数据
        var content = new SpiderContent_HighQualityQA(originalTitle, mainContent.ToString(), sourceFrom);
        return Task.FromResult(content);
    }
}