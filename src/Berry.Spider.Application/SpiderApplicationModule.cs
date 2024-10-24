﻿using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Berry.Spider.Application;

[DependsOn(
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(SpiderApplicationContractsModule),
    typeof(AbpAutoMapperModule))]
public class SpiderApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<SpiderApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SpiderApplicationModule>(validate: true);
        });
        
        context.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(SpiderApplicationModule).Assembly);
        });
    }
}