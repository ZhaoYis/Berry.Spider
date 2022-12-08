using DotXxlJob.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.XxlJob;

public class XxlJobExecutorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly XxlRestfulServiceHandler _rpcService;

    public XxlJobExecutorMiddleware(IServiceProvider provider, RequestDelegate next)
    {
        this._rpcService = provider.GetRequiredService<XxlRestfulServiceHandler>();
        this._next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string? contentType = context.Request.ContentType;

        if ("POST".Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(contentType)
            && contentType.ToLower().StartsWith("application/json"))
        {
            await _rpcService.HandlerAsync(context.Request, context.Response);
            return;
        }

        await _next.Invoke(context);
    }
}