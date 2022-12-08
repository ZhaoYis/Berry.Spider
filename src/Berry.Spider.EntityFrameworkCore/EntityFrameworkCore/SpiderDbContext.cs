using Berry.Spider.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Berry.Spider.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class SpiderDbContext : AbpDbContext<SpiderDbContext>
{
    public DbSet<SpiderContent> SpiderContents { get; set; }
    public DbSet<SpiderContent_Composition> SpiderContentCompositions { get; set; }
    public DbSet<SpiderContent_HighQualityQA> SpiderContentHighQualityQas { get; set; }
    public DbSet<SpiderTitleContent> SpiderTitleContents { get; set; }
    public DbSet<SpiderBasic> SpiderBasics { get; set; }

    public SpiderDbContext(DbContextOptions<SpiderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //基础表
        builder.Entity<SpiderContent>(b =>
        {
            b.ToTable("Content_20221208");
            //b.ToTable("Content_Composition");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        //作文
        builder.Entity<SpiderContent_Composition>(b =>
        {
            b.ToTable("Content_Composition");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        //优质问答
        builder.Entity<SpiderContent_HighQualityQA>(b =>
        {
            b.ToTable("Content_HighQualityQA");

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