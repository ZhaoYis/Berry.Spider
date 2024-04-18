using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi.Models;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace Berry.Spider.Gateway.Ocelot.Host;

[DependsOn(typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule))]
public class SpiderGatewayOcelotHostModule : AbpModule
{
    private const string DefaultCorsPolicyName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services
            .AddOcelot(configuration)
            //服务发现
            .AddConsul()
            //缓存
            .AddCacheManager(x => { x.WithDictionaryHandle(); })
            //服务质量控制
            .AddPolly()
            //委托处理程序
            .AddDelegatingHandler<LoggerDelegatingHandler>(true);

        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("gateway", new OpenApiInfo { Title = "Berry.Spider网关服务", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
        context.Services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicyName, builder =>
            {
                builder
#if DEBUG
                    .SetIsOriginAllowed(_ => true)
#else
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
#endif
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger(c => { c.RouteTemplate = "/api-docs/{documentName}/swagger.json"; });
            app.UseAbpSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api-docs/gateway/swagger.json", "Berry.Spider.Gateway.Ocelot.Host v1");
                c.SwaggerEndpoint("/api-docs/berry_spider_api/swagger.json", "Berry.Spider.HttApi.Host v1");

                c.DocExpansion(DocExpansion.None);
                c.RoutePrefix = string.Empty;
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseCorrelationId();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors(DefaultCorsPolicyName);

        app.UseOcelot().Wait();
        
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}