using Berry.Spider.Domain;
using Dapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.Dapper;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderContentDapperRepository : DapperRepository<SpiderDbContext>, ITransientDependency
{
    public SpiderContentDapperRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
    }

    public async Task<IEnumerable<SpiderContent>> GetAllAsync()
    {
        var db = await this.GetDbConnectionAsync();

        var result = await db.QueryAsync<SpiderContent>(@"
select A.标题 as Title,
       A.内容 as Content
from Content as A
where A.已发 = 1 order by 时间 asc;",
            transaction: this.DbTransaction, commandTimeout: 1000);

        return result;
    }
}