using Berry.Spider.Domain;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderContentCompositionRepository : EfCoreRepository<SpiderDbContext, SpiderContent_Composition, int>, ISpiderContentCompositionRepository
{
    public SpiderContentCompositionRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}