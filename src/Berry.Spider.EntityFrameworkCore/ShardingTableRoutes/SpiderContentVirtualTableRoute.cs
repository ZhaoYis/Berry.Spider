using Berry.Spider.Domain;
using ShardingCore.Core.EntityMetadatas;
using ShardingCore.VirtualRoutes.Months;

namespace Berry.Spider.EntityFrameworkCore.ShardingTableRoutes;

public class SpiderContentVirtualTableRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<SpiderContent>
{
    /// <summary>
    /// 配置分表的一些信息
    /// 1.ShardingProperty 哪个字段分表
    /// 2.TableSeparator 分表的后缀和表名的连接符
    /// 3.AutoCreateTable 启动时是否需要创建对应的分表信息
    /// 3.ShardingExtraProperty 额外分片字段
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(EntityMetadataTableBuilder<SpiderContent> builder)
    {
        builder.ShardingProperty(o => o.Time);
    }

    /// <summary>是否需要自动创建按时间分表的路由</summary>
    /// <returns></returns>
    public override bool AutoCreateTableByTime()
    {
        return true;
    }

    public override DateTime GetBeginTime()
    {
        return new DateTime(2024, 1, 1);
    }
}