using Berry.Spider.Application;
using Berry.Spider.AspNetCore.Mvc;
using Berry.Spider.Baidu;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.EventBus.MongoDB;
using Berry.Spider.EventBus.RabbitMq;
using Berry.Spider.OpenAI.Application;
using Berry.Spider.Segmenter.JiebaNet;
using Berry.Spider.Sogou;
using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace Berry.Spider.HttpApi.Host;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(SpiderHttpApiModule),
    typeof(SpiderEntityFrameworkCoreModule),
    typeof(SpiderAspNetCoreMvcModule),
    typeof(SpiderEventBusRabbitMqModule),
    typeof(SpiderEventBusMongoDBModule),
    typeof(SpiderSegmenterJiebaNetModule),
    typeof(TouTiaoSpiderApplicationModule),
    typeof(BaiduSpiderApplicationModule),
    typeof(SogouSpiderApplicationModule),
    typeof(SpiderApplicationModule),
    typeof(SpiderOpenAIApplicationModule)
)]
public class SpiderHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Spider API", Version = "v1"});
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });

        Configure<AbpAntiForgeryOptions>(opt => { opt.AutoValidate = false; });

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Configure<AbpJsonOptions>(opt =>
        // {
        //     opt.OutputDateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";
        // });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Spider API");
            options.DocExpansion(DocExpansion.None);
        });
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}