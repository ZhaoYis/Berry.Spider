using System;
using Berry.Spider.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
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