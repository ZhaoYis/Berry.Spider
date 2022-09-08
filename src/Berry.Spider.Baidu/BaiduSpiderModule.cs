﻿using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.Baidu;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(BaiduSpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule)
)]
public class BaiduSpiderModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        //注入百度爬虫提供者
        context.Services.AddTransient<BaiduSpider4RelatedSearchProvider>();

        return Task.CompletedTask;
    }
}