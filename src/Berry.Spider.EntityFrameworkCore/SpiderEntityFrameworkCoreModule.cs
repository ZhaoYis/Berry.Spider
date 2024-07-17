using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Dapper;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Berry.Spider.EntityFrameworkCore;

[DependsOn(
    typeof(SpiderDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpDapperModule))]
public class SpiderEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //添加默认仓储
        context.Services.AddAbpDbContext<SpiderDbContext>();
        context.Services.AddAbpDbContext<SpiderDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            //自定义仓储
            options.AddRepository<SpiderContent, SpiderContentRepository>();
            options.AddRepository<SpiderContent_HighQualityQA, SpiderContentHighQualityQARepository>();
            options.AddRepository<SpiderContent_Composition, SpiderContentCompositionRepository>();
            options.AddRepository<SpiderContent_Title, SpiderTitleContentRepository>();
            options.AddRepository<SpiderBasicInfo, SpiderBasicInfoRepository>();
            options.AddRepository<WeatherForecast, WeatherForecastRepository>();
        });

        //添加默认仓储
        context.Services.AddAbpDbContext<SpiderBizDbContext>();
        context.Services.AddAbpDbContext<SpiderBizDbContext>(options =>
        {
            options.AddRepository<SpiderAppInfo, SpiderAppInfoRepository>();
            options.AddRepository<ServMachineInfo, ServMachineInfoRepository>();
            options.AddRepository<ServMachineGroupInfo, ServMachineGroupInfoRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also SpiderDbContextFactory for EF Core tooling. */
            options.UseMySQL();
        });
    }
}