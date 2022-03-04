using Berry.Spider.Domain;
using Berry.Spider.Domain.TouTiao;
using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Berry.Spider.EntityFrameworkCore.TouTiao;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Berry.Spider.EntityFrameworkCore;

[DependsOn(
    typeof(SpiderDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class SpiderEntityFrameworkCoreModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SpiderDbContext>();

        //添加默认仓储
        context.Services.AddAbpDbContext<SpiderDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            //自定义仓储
            options.AddRepository<TouTiaoSpiderContent, TouTiaoSpiderRepository>();
        });
        
        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also SpiderDbContextFactory for EF Core tooling. */
            options.UseMySQL();
        });
        
        return Task.CompletedTask;
    }
}