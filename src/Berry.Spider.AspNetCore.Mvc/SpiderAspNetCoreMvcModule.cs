﻿using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Berry.Spider.AspNetCore.Mvc
{
    public class SpiderAspNetCoreMvcModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<MvcOptions>(mvcOptions =>
            {
                //配置自定义过滤器
                mvcOptions.Filters.Add<ApiContentActionFilter>();
            });
        }
    }
}