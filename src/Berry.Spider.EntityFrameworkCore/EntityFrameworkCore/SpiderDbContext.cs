using Berry.Spider.Domain.TouTiao;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class SpiderDbContext: AbpDbContext<SpiderDbContext>
{
    public DbSet<TouTiaoSpiderContent> TouTiaoSpiderContents { get; set; }
    
    public SpiderDbContext(DbContextOptions<SpiderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<TouTiaoSpiderContent>(b =>
        {
            b.ToTable("Content");
        
            //Configure the base properties
            b.ConfigureByConvention();
        });
    }
}