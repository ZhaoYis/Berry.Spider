using Microsoft.Extensions.Options;
using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Scriban;

namespace Berry.Spider.Core;

/// <summary>
/// 自定义模版引擎
/// </summary>
public class CustomTemplateDefinitionProvider : TemplateDefinitionProvider
{
    private TitleTemplateContentOptions TitleTemplateContentOptions { get; }
    private AbstractTemplateOptions AbstractTemplateOptions { get; }

    public CustomTemplateDefinitionProvider(IOptionsSnapshot<TitleTemplateContentOptions> titleTemplateContentOptions,
        IOptionsSnapshot<AbstractTemplateOptions> abstractTemplateOptions)
    {
        this.TitleTemplateContentOptions = titleTemplateContentOptions.Value;
        this.AbstractTemplateOptions = abstractTemplateOptions.Value;
    }

    public override void Define(ITemplateDefinitionContext context)
    {
        //标题模版
        if (this.TitleTemplateContentOptions is { IsEnableFormatTitle: true } &&
            this.TitleTemplateContentOptions.Templates.Count > 0)
        {
            List<string> titleTemplateNames =
                this.TitleTemplateContentOptions.Templates.Select(c => c.Name).ToList();
            foreach (string name in titleTemplateNames)
            {
                context.Add(new TemplateDefinition(name).WithScribanEngine());
            }
        }

        //摘要模版
        if (this.AbstractTemplateOptions is { IsEnableAbstract: true } &&
            this.AbstractTemplateOptions.Templates.Count > 0)
        {
            List<string> abstractTemplateNames =
                this.AbstractTemplateOptions.Templates.Select(c => c.Name).ToList();
            foreach (string name in abstractTemplateNames)
            {
                context.Add(new TemplateDefinition(name).WithScribanEngine());
            }
        }
    }
}