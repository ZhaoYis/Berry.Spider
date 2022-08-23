using System.Linq.Expressions;
using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderContentRepository : EfCoreRepository<SpiderDbContext, SpiderContent, int>, ISpiderContentRepository
{
    public SpiderContentRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<int> MyCountAsync(Expression<Func<SpiderContent, bool>> predicate)
    {
        var db = await this.GetDbContextAsync();
        
        return await db.SpiderContents.Where(predicate).CountAsync();
    }
}