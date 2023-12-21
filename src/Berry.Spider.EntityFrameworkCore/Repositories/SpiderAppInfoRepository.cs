using Berry.Spider.Domain;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderAppInfoRepository : EfCoreRepository<SpiderBizDbContext, SpiderAppInfo, int>, ISpiderAppInfoRepository
{
    public SpiderAppInfoRepository(IDbContextProvider<SpiderBizDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}