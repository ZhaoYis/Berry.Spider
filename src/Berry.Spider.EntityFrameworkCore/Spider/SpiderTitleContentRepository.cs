using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderTitleContentRepository : EfCoreRepository<SpiderDbContext, SpiderTitleContent, int>,
    ISpiderTitleContentRepository
{
    public SpiderTitleContentRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}