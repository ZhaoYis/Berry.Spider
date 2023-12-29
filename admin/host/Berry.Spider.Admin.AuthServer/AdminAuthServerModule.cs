using System;
using System.IO;
using System.Linq;
using Berry.Spider.Admin.EntityFrameworkCore;
using Berry.Spider.Admin.Localization;
using Berry.Spider.Admin.MultiTenancy;
using Localization.Resources.AbpUi;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace Berry.Spider.Admin;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AdminEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpOpenIddictAspNetCoreModule)
)]
public class AdminAuthServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences(new[] { "Admin", "Spider" });
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDev())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.UpdateAbpClaimTypes = true;
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(30));
                serverBuilder.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
                serverBuilder.SetIdentityTokenLifetime(TimeSpan.FromMinutes(30));
                serverBuilder.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

                string filePath = Path.Combine(AppContext.BaseDirectory, "berry-authserver.dsx.plus.pfx");
                serverBuilder.AddProductionEncryptionAndSigningCertificate(filePath, configuration["StringEncryption:SSLPassPhrase"]);
            });
        }
        else
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.UpdateAbpClaimTypes = true;
                options.AddDevelopmentEncryptionAndSigningCertificate = true;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddDevelopmentEncryptionCertificate();
                serverBuilder.AddDevelopmentSigningCertificate();
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AdminResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource),
                    typeof(AccountResource)
                );
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle => { bundle.AddFiles("/global-styles.css"); }
            );
        });

        Configure<AbpAuditingOptions>(options =>
        {
            //options.IsEnabledForGetRequests = true;
            options.ApplicationName = "AuthServer";
        });

        if (hostingEnvironment.IsDev())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<AdminDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}Berry.Spider.Admin.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<AdminDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}Berry.Spider.Admin.Domain"));
            });
        }

        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());

            options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
        });

        Configure<AbpBackgroundJobOptions>(options => { options.IsJobExecutionEnabled = false; });

        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "Berry:Admin:"; });

        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Admin");
        if (!hostingEnvironment.IsDev())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Admin-Protection-Keys");
        }

        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });

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

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options => { options.IsDynamicClaimsEnabled = true; });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDev())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDev())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}