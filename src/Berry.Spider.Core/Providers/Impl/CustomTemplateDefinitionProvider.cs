using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Scriban;

namespace Berry.Spider.Core;

/// <summary>
/// 自定义模版引擎
/// </summary>
public class CustomTemplateDefinitionProvider : TemplateDefinitionProvider
{
    public override void Define(ITemplateDefinitionContext context)
    {
        string[] names = Enum.GetNames<TitleTemplateNames>();
        foreach (string name in names)
        {
            context.Add(new TemplateDefinition(name).WithScribanEngine());
        }
    }
}