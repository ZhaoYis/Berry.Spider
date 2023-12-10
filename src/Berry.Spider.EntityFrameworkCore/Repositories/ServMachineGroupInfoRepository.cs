using Berry.Spider.Domain;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class ServMachineGroupInfoRepository : EfCoreRepository<SpiderBizDbContext, ServMachineGroupInfo, int>, IServMachineGroupInfoRepository
{
    public ServMachineGroupInfoRepository(IDbContextProvider<SpiderBizDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}