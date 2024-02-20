using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore.ShardingTableRoutes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;
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
        var configuration = context.Services.GetConfiguration();

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

        //额外添加分片配置
        context.Services.AddShardingDbContext<SpiderDbContext>()
            .UseRouteConfig(op =>
            {
                //自定义分表路由规则
                op.AddShardingTableRoute<SpiderContentVirtualTableRoute>();
            })
            .UseConfig(op =>
            {
                op.UseShardingQuery((connStr, builder) =>
                {
                    //connStr is delegate input param
                    builder.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
                });

                op.UseShardingTransaction((connection, builder) =>
                {
                    //connection is delegate input param
                    builder.UseMySql(connection, ServerVersion.Parse(connection.ServerVersion));
                });

                //use your data base connection string
                op.AddDefaultDataSource("berry_spider_v2", configuration.GetConnectionString("Default"));
            }).AddShardingCore();
    }
}