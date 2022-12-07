using Berry.Spider.XxlJob;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseXxlJobExecutor(this IApplicationBuilder app)
    {
        return app.UseMiddleware<XxlJobExecutorMiddleware>();
    }
}