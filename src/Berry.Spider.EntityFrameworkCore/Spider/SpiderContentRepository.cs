using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderContentRepository : EfCoreRepository<SpiderDbContext, SpiderContent, int>, ISpiderContentRepository
{
    public SpiderContentRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}