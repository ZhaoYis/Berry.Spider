using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.Consumers.HttpApi;

public class SpiderConsumerHttpApiService : ISpiderConsumerHttpApiService
{
    public async Task InitAsync()
    {
        string? assemblyName = typeof(SpiderConsumerHttpApiService).Assembly.GetName().Name;
        if (!string.IsNullOrEmpty(assemblyName))
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllers().AddApplicationPart(Assembly.Load(assemblyName));
                
            var app = builder.Build();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            await app.RunAsync();
        }
    }
}