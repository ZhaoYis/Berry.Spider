using AutoMapper;
using Berry.Spider.Domain;
using Volo.Abp.AutoMapper;

namespace Berry.Spider;

public class SpiderApplicationAutoMapperProfile : Profile
{
    public SpiderApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<SpiderBasic, SpiderDto>();
        CreateMap<SpiderCreateInput, SpiderBasic>()
            .Ignore(c => c.Id)
            .Ignore(c => c.CreationTime)
            .Ignore(c => c.LastModificationTime)
            .Ignore(c => c.IsDeleted)
            .Ignore(c => c.ExtraProperties)
            .Ignore(c => c.ConcurrencyStamp);
        CreateMap<SpiderUpdateInput, SpiderBasic>()
            .Ignore(c => c.Id)
            .Ignore(c => c.CreationTime)
            .Ignore(c => c.LastModificationTime)
            .Ignore(c => c.IsDeleted)
            .Ignore(c => c.ExtraProperties)
            .Ignore(c => c.ConcurrencyStamp);
    }
}