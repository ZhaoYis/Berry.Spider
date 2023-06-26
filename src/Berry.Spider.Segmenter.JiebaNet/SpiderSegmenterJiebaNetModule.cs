using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.Segmenter.JiebaNet;

[DependsOn(typeof(SpiderSegmenterModule))]
public class SpiderSegmenterJiebaNetModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<ISegmenterProvider, JiebaNetSegmenterProvider>();
    }
}