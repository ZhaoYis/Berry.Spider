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
    private IOptionsSnapshot<TitleTemplateContentOptions> Options { get; }

    public CustomTemplateContentProvider(IOptionsSnapshot<TitleTemplateContentOptions> options)
    {
        this.Options = options;
    }

    public Task<string> GetOrNullAsync(TemplateContentContributorContext context)
    {
        var templateName = context.TemplateDefinition.Name;

        if (this.Options.Value.Templates.Count > 0)
        {
            NameValue? nameValue = this.Options.Value.Templates.FirstOrDefault(c => c != null && c.Name.Equals(templateName));
            if (nameValue != null)
            {
                return Task.FromResult(nameValue.Value);
            }
        }

        return Task.FromResult<string>("{{model.original_title}}");
    }
}