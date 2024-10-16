using Berry.Spider.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Berry.Spider.EntityFrameworkCore;

[ConnectionStringName("BizDb")]
public class SpiderBizDbContext : AbpDbContext<SpiderBizDbContext>
{
    private const string TableNamePrefix = "spider_";

    public DbSet<SpiderAppInfo> SpiderAppInfos { get; set; }

    public DbSet<ServMachineInfo> ServMachineInfos { get; set; }
    public DbSet<ServMachineGroupInfo> ServMachineGroupInfos { get; set; }

    public SpiderBizDbContext(DbContextOptions<SpiderBizDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SpiderAppInfo>(b =>
        {
            b.ToTable($"{TableNamePrefix}app_info");
            b.Property(p => p.BizNo).IsRequired().HasMaxLength(32);
            b.Property(p => p.Name).IsRequired().HasMaxLength(64);
            b.Property(p => p.TagName).IsRequired().HasMaxLength(64);
            b.Property(p => p.Version).IsRequired().HasMaxLength(64);
            b.Property(p => p.TargetCommitish).HasMaxLength(128);
            b.Property(p => p.OssKey).HasMaxLength(256);

            b.HasIndex(i => i.BizNo);

            //Configure the base properties
            b.ConfigureByConvention();
        });

        builder.Entity<ServMachineInfo>(b =>
        {
            b.ToTable($"{TableNamePrefix}serv_machine_info");
            b.Property(p => p.BizNo).IsRequired().HasMaxLength(32);
            b.Property(p => p.GroupCode).IsRequired().HasMaxLength(32);
            b.Property(p => p.MachineName).IsRequired().HasMaxLength(64);
            b.Property(p => p.MachineCode).IsRequired().HasMaxLength(64);
            b.Property(p => p.MachineIpAddr).IsRequired().HasMaxLength(128);
            b.Property(p => p.ConnectionId).HasMaxLength(128);
            b.Property(p => p.MachineCode).HasMaxLength(128);

            b.HasIndex(i => i.BizNo);
            b.HasIndex(i => i.GroupCode);
            b.HasIndex(i => i.MachineName);

            //Configure the base properties
            b.ConfigureByConvention();
        });

        builder.Entity<ServMachineGroupInfo>(b =>
        {
            b.ToTable($"{TableNamePrefix}serv_machine_group_info");
            b.Property(p => p.BizNo).IsRequired().HasMaxLength(32);
            b.Property(p => p.Code).IsRequired().HasMaxLength(64);
            b.Property(p => p.Name).IsRequired().HasMaxLength(64);
            b.Property(p => p.Remark).HasMaxLength(256);

            b.HasIndex(i => i.BizNo);

            //Configure the base properties
            b.ConfigureByConvention();
        });
    }
}