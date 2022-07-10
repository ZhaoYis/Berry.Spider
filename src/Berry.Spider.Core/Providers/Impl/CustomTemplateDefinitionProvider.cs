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
    private IOptionsSnapshot<TitleTemplateContentOptions> Options { get; }

    public CustomTemplateDefinitionProvider(IOptionsSnapshot<TitleTemplateContentOptions> options)
    {
        this.Options = options;
    }

    public override void Define(ITemplateDefinitionContext context)
    {
        List<string> names = this.Options.Value.Templates.Select(c => c.Name).ToList();
        foreach (string name in names)
        {
            context.Add(new TemplateDefinition(name).WithScribanEngine());
        }
    }
}