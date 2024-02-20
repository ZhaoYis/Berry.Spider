using System.Collections.Immutable;
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
    private TitleTemplateContentOptions TitleTemplateContentOptions { get; }
    private AbstractTemplateOptions AbstractTemplateOptions { get; }

    public CustomTemplateContentProvider(IOptionsSnapshot<TitleTemplateContentOptions> titleTemplateContentOptions,
        IOptionsSnapshot<AbstractTemplateOptions> abstractTemplateOptions)
    {
        this.TitleTemplateContentOptions = titleTemplateContentOptions.Value;
        this.AbstractTemplateOptions = abstractTemplateOptions.Value;
    }

    public Task<string> GetOrNullAsync(TemplateContentContributorContext context)
    {
        var templateName = context.TemplateDefinition.Name;

        ImmutableList<NameValue> templates = ImmutableList.Create<NameValue>();

        //标题模版
        if (this.TitleTemplateContentOptions is {IsEnableFormatTitle: true, Templates.Count: > 0})
        {
            templates = templates.AddRange(this.TitleTemplateContentOptions.Templates);
        }

        //摘要模版
        if (this.AbstractTemplateOptions is {IsEnableAbstract: true, Templates.Count: > 0})
        {
            templates = templates.AddRange(this.AbstractTemplateOptions.Templates);
        }

        NameValue? nameValue = templates.FirstOrDefault(c => !string.IsNullOrEmpty(c.Name) && c.Name.Equals(templateName));
        if (nameValue is not null)
        {
            return Task.FromResult(nameValue.Value);
        }

        return Task.FromResult("{{model.original_title}}");
    }
}