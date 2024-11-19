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
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;

namespace Berry.Spider.HttpApi.Host;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
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

        ConfigureAuthentication(context, configuration);
        ConfigureCache(configuration);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
        Configure<AbpAntiForgeryOptions>(opt => { opt.AutoValidate = false; });
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        //分布式缓存
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "Berry:Spider:";
            options.GlobalCacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(10);
            options.GlobalCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        });
        //分布式缓存Redis
        // Configure<RedisCacheOptions>(options => { options.InstanceName = Assembly.GetExecutingAssembly().FullName; });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
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
    }

    private void ConfigureDistributedLocking(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }

    private void ConfigureDataProtection(ServiceConfigurationContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Spider");
        if (!hostingEnvironment.IsDev())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Spider-Protection-Keys");
        }
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
                options.Audience = "Spider";
            });

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options => { options.IsDynamicClaimsEnabled = true; });
    }

    private void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        // context.Services.AddAbpSwaggerGen(options =>
        //     {
        //         options.SwaggerDoc("v1", new OpenApiInfo { Title = "Spider API", Version = "v1" });
        //         options.DocInclusionPredicate((docName, description) => true);
        //         options.CustomSchemaIds(type => type.FullName);
        //     });

        context.Services.AddAbpSwaggerGenWithOAuth(configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                { "Spider", "Spider API" }
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Spider API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDev())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Spider API");
            options.DocExpansion(DocExpansion.None);
            
            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("Spider");
        });

        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}