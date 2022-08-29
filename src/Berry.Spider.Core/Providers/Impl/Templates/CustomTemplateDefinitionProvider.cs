using Berry.Spider.Contracts;
using Microsoft.Extensions.Options;
using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Scriban;

namespace Berry.Spider.Core;

/// <summary>
/// 自定义模版引擎
/// </summary>
public class CustomTemplateDefinitionProvider : TemplateDefinitionProvider
{
    private IOptionsSnapshot<TitleTemplateContentOptions> TitleTemplateContentOptions { get; }
    private IOptionsSnapshot<AbstractTemplateOptions> AbstractTemplateOptions { get; }

    public CustomTemplateDefinitionProvider(IOptionsSnapshot<TitleTemplateContentOptions> titleTemplateContentOptions,
        IOptionsSnapshot<AbstractTemplateOptions> abstractTemplateOptions)
    {
        this.TitleTemplateContentOptions = titleTemplateContentOptions;
        this.AbstractTemplateOptions = abstractTemplateOptions;
    }

    public override void Define(ITemplateDefinitionContext context)
    {
        //标题模版
        if (this.TitleTemplateContentOptions.Value is {IsEnableFormatTitle: true} &&
            this.TitleTemplateContentOptions.Value.Templates.Count > 0)
        {
            List<string> titleTemplateNames =
                this.TitleTemplateContentOptions.Value.Templates.Select(c => c.Name).ToList();
            foreach (string name in titleTemplateNames)
            {
                context.Add(new TemplateDefinition(name).WithScribanEngine());
            }
        }

        //摘要模版
        if (this.AbstractTemplateOptions.Value is {IsEnableAbstract: true} &&
            this.AbstractTemplateOptions.Value.Templates.Count > 0)
        {
            List<string> abstractTemplateNames =
                this.AbstractTemplateOptions.Value.Templates.Select(c => c.Name).ToList();
            foreach (string name in abstractTemplateNames)
            {
                context.Add(new TemplateDefinition(name).WithScribanEngine());
            }
        }
    }
}