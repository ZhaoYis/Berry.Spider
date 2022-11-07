using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Dapper;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Berry.Spider.EntityFrameworkCore;

[DependsOn(
    typeof(SpiderDomainModule),
    typeof(AbpEntityFrameworkCoreModule), typeof(AbpDapperModule))]
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
            options.AddRepository<SpiderContent, SpiderContentRepository>();
            options.AddRepository<SpiderContent_HighQualityQA, SpiderContentHighQualityQARepository>();
            options.AddRepository<SpiderContent_Composition, SpiderContentCompositionRepository>();
            options.AddRepository<SpiderTitleContent, SpiderTitleContentRepository>();
            options.AddRepository<SpiderBasic, SpiderBasicRepository>();
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