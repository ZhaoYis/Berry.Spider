using Berry.Spider.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderTitleContentRepository : EfCoreRepository<SpiderDbContext, SpiderContent_Title, int>,
    ISpiderContentTitleRepository
{
    public SpiderTitleContentRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<int> MyCountAsync(Expression<Func<SpiderContent_Title, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();
        return await db.SpiderContentTitles.CountAsync(predicate, cancellationToken);
    }
}