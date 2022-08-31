using System.Linq.Expressions;
using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderTitleContentRepository : EfCoreRepository<SpiderDbContext, SpiderTitleContent, int>,
    ISpiderTitleContentRepository
{
    public SpiderTitleContentRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<int> MyCountAsync(Expression<Func<SpiderTitleContent, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();
        return await db.SpiderTitleContents.CountAsync(predicate, cancellationToken);
    }
}