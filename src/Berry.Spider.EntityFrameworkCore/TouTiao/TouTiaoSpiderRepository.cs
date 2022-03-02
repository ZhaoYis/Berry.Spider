using Berry.Spider.Domain.TouTiao;
using Berry.Spider.EntityFrameworkCore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore.TouTiao;

public class TouTiaoSpiderRepository : EfCoreRepository<SpiderDbContext, TouTiaoSpiderContent, int>,
    ITouTiaoSpiderRepository
{
    public TouTiaoSpiderRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}