﻿using AutoMapper;
using Berry.Spider.Biz;
using Berry.Spider.Common;
using Berry.Spider.Domain;
using Berry.Spider.Weather;
using Volo.Abp.AutoMapper;

namespace Berry.Spider.Application;

public class SpiderApplicationAutoMapperProfile : Profile
{
    public SpiderApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<SpiderBasicInfo, SpiderDto>();
        CreateMap<SpiderCreateInput, SpiderBasicInfo>()
            .Ignore(c => c.Id)
            .Ignore(c => c.CreationTime)
            .Ignore(c => c.LastModificationTime)
            .Ignore(c => c.IsDeleted)
            .Ignore(c => c.ExtraProperties)
            .Ignore(c => c.ConcurrencyStamp);
        CreateMap<SpiderUpdateInput, SpiderBasicInfo>()
            .Ignore(c => c.Id)
            .Ignore(c => c.CreationTime)
            .Ignore(c => c.LastModificationTime)
            .Ignore(c => c.IsDeleted)
            .Ignore(c => c.ExtraProperties)
            .Ignore(c => c.ConcurrencyStamp);

        CreateMap<ApplicationLifetimeData, ApplicationLifetimeDto>();

        CreateMap<ServMachineInfo, ServMachineDto>();
        CreateMap<ServMachineDto, ServMachineInfo>();
        CreateMap<ServMachineOnlineDto, ServMachineInfo>()
            .Ignore(c => c.Id)
            .Ignore(c => c.BizNo)
            .Ignore(c => c.Status)
            .Ignore(c => c.LastOnlineTime)
            .IgnoreFullAuditedObjectProperties();

        CreateMap<SpiderAppInfo, SpiderAppInfoDto>();

        CreateMap<WeatherForecastByDate, WeatherForecast>()
            .Ignore(c => c.Id)
            .Ignore(c => c.ExtraProperties)
            .Ignore(c => c.ConcurrencyStamp);
    }
}