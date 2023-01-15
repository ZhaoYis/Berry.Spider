using System;
using System.Linq;
using Berry.Spider.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderContentKeywordRepository : EfCoreRepository<SpiderDbContext, SpiderContent_Keyword, int>,
    ISpiderContentKeywordRepository
{
    public SpiderContentKeywordRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
    }

    public async Task<int> MyCountAsync(Expression<Func<SpiderContent_Keyword, bool>> predicate)
    {
        var db = await this.GetDbContextAsync();

        return await db.SpiderContentKeywords.Where(predicate).CountAsync();
    }
}