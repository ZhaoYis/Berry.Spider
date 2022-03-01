using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class SpiderDbContext: AbpDbContext<SpiderDbContext>
{
    public SpiderDbContext(DbContextOptions<SpiderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // builder.Entity<Book>(b =>
        // {
        //     b.ToTable("Books");
        //
        //     //Configure the base properties
        //     b.ConfigureByConvention();
        //
        //     //Configure other properties (if you are using the fluent API)
        //     b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        // });
    }
}