using Berry.Spider.Contracts;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TextTemplating;

namespace Berry.Spider.Core;

/// <summary>
/// 自定义模板内容贡献者
/// </summary>
public class CustomTemplateContentProvider : ITemplateContentContributor, ITransientDependency
{
    private IOptionsSnapshot<TitleTemplateContentOptions> TitleTemplateContentOptions { get; }
    private IOptionsSnapshot<AbstractTemplateOptions> AbstractTemplateOptions { get; }

    public CustomTemplateContentProvider(IOptionsSnapshot<TitleTemplateContentOptions> titleTemplateContentOptions,
        IOptionsSnapshot<AbstractTemplateOptions> abstractTemplateOptions)
    {
        this.TitleTemplateContentOptions = titleTemplateContentOptions;
        this.AbstractTemplateOptions = abstractTemplateOptions;
    }

    public Task<string> GetOrNullAsync(TemplateContentContributorContext context)
    {
        var templateName = context.TemplateDefinition.Name;

        List<NameValue> templates = new List<NameValue>();

        //标题模版
        if (this.TitleTemplateContentOptions.Value is { IsEnableFormatTitle: true } &&
            this.TitleTemplateContentOptions.Value.Templates.Count > 0)
        {
            templates.AddRange(this.TitleTemplateContentOptions.Value.Templates);
        }

        //摘要模版
        if (this.AbstractTemplateOptions.Value is { IsEnableAbstract: true } &&
            this.AbstractTemplateOptions.Value.Templates.Count > 0)
        {
            templates.AddRange(this.AbstractTemplateOptions.Value.Templates);
        }

        NameValue nameValue = templates.First(c => c.Name.Equals(templateName));
        return Task.FromResult(nameValue.Value);
    }
}