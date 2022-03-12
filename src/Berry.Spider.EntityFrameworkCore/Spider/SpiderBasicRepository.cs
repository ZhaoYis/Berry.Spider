using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class SpiderBasicRepository : EfCoreRepository<SpiderDbContext, SpiderBasic, int>, ISpiderBasicRepository
{
    public SpiderBasicRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}