using Volo.Abp.Modularity;

namespace Berry.Spider.XxlJob.JobHandler;

[DependsOn(typeof(SpiderXxlJobModule))]
public class SpiderXxlJobHandlerModule : AbpModule
{
}