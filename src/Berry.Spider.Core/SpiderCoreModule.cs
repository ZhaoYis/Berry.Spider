using Berry.Spider.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.Core;

public class SpiderCoreModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //配置WebDriverOptions
        context.Services.Configure<WebDriverOptions>(configuration.GetSection(nameof(WebDriverOptions)));

        //注入文本解析器
        context.Services.AddTransient<TouTiaoQuestionTextAnalysisProvider>();
        context.Services.AddTransient<BaiduRelatedSearchTextAnalysisProvider>();

        return Task.CompletedTask;
    }
}