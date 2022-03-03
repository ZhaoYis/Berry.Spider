using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;

public class SpiderDbContextFactory: IDesignTimeDbContextFactory<SpiderDbContext>
{
    public SpiderDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<SpiderDbContext>()
            .UseSqlite(configuration.GetConnectionString("Default"));

        return new SpiderDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Berry.Spider.Consumers/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}