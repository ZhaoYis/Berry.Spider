using Berry.Spider.Domain;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class ServMachineInfoRepository : EfCoreRepository<SpiderBizDbContext, ServMachineInfo, int>, IServMachineInfoRepository
{
    public ServMachineInfoRepository(IDbContextProvider<SpiderBizDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}