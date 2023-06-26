using Berry.Spider.Domain;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderBasicInfoRepository : EfCoreRepository<SpiderDbContext, SpiderBasicInfo, int>, ISpiderBasicInfoRepository
{
    public SpiderBasicInfoRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}