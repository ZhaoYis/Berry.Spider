using Berry.Spider.Domain;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderContentHighQualityQARepository : EfCoreRepository<SpiderDbContext, SpiderContent_HighQualityQA, int>, ISpiderContentHighQualityQARepository
{
    public SpiderContentHighQualityQARepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}