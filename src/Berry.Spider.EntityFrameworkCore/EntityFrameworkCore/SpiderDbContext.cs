using Berry.Spider.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class SpiderDbContext : AbpDbContext<SpiderDbContext>
{
    public DbSet<SpiderContent> SpiderContents { get; set; }
    public DbSet<SpiderTitleContent> SpiderTitleContents { get; set; }
    public DbSet<SpiderBasic> SpiderBasics { get; set; }

    public SpiderDbContext(DbContextOptions<SpiderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SpiderContent>(b =>
        {
            b.ToTable("Content_20220901");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        builder.Entity<SpiderTitleContent>(b =>
        {
            b.ToTable("TitleContent");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        builder.Entity<SpiderBasic>(b =>
        {
            b.ToTable("SpiderBasic");

            //Configure the base properties
            b.ConfigureByConvention();
        });
    }
}