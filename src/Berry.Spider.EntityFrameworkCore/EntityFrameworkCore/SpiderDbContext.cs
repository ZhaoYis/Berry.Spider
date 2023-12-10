using Berry.Spider.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Berry.Spider.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class SpiderDbContext : AbpDbContext<SpiderDbContext>
{
    private const string TableNamePrefix = "spider_";

    public DbSet<SpiderContent> SpiderContents { get; set; }
    public DbSet<SpiderContent_Keyword> SpiderContentKeywords { get; set; }
    public DbSet<SpiderContent_Composition> SpiderContentCompositions { get; set; }
    public DbSet<SpiderContent_HighQualityQA> SpiderContentHighQualityQas { get; set; }
    public DbSet<SpiderContent_Title> SpiderContentTitles { get; set; }
    public DbSet<SpiderBasicInfo> SpiderBasicInfos { get; set; }

    public SpiderDbContext(DbContextOptions<SpiderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //基础表
        builder.Entity<SpiderContent>(b =>
        {
            b.ToTable($"{TableNamePrefix}content");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        //作文
        builder.Entity<SpiderContent_Composition>(b =>
        {
            b.ToTable($"{TableNamePrefix}content_composition");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        //优质问答
        builder.Entity<SpiderContent_HighQualityQA>(b =>
        {
            b.ToTable($"{TableNamePrefix}content_high_quality_qa");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        //根据关键字抓取的一级页面标题信息
        builder.Entity<SpiderContent_Keyword>(b =>
        {
            b.ToTable($"{TableNamePrefix}content_keyword");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        builder.Entity<SpiderContent_Title>(b =>
        {
            b.ToTable($"{TableNamePrefix}content_title");

            //Configure the base properties
            b.ConfigureByConvention();
        });

        builder.Entity<SpiderBasicInfo>(b =>
        {
            b.ToTable($"{TableNamePrefix}basic_info");

            //Configure the base properties
            b.ConfigureByConvention();
        });
    }
}