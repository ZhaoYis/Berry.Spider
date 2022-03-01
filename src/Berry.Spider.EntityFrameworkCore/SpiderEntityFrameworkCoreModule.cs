using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Berry.Spider.EntityFrameworkCore;

[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
public class SpiderEntityFrameworkCoreModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SpiderDbContext>();

        //添加默认仓储
        context.Services.AddAbpDbContext<SpiderDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        return Task.CompletedTask;
    }
}