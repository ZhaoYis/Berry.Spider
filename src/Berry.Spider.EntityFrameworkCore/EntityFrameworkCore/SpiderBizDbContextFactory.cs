using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;

public class SpiderBizDbContextFactory : IDesignTimeDbContextFactory<SpiderBizDbContext>
{
    public SpiderBizDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<SpiderBizDbContext>()
            .UseMySql(configuration.GetConnectionString("BizDb"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("BizDb")));

        return new SpiderBizDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../host/Berry.Spider.HttpApi.Host"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}