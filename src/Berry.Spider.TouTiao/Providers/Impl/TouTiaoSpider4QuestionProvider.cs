using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：问答
/// </summary>
[Dependency(ServiceLifetime.Transient), ExposeServices(typeof(ITouTiaoSpiderProvider))]
public class TouTiaoSpider4QuestionProvider : ITouTiaoSpiderProvider
{
    public Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}