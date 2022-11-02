using System.Threading.Tasks;
using Berry.Spider.Abstractions;
using Berry.Spider.TouTiao;
using DotNetCore.CAP;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条：问答
/// </summary>
public class TouTiaoSpider4QuestionEventHandler : ICapSubscribe
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(TouTiaoSpider4QuestionPushEto.EventNameString)]
    public async Task HandleEventAsync(TouTiaoSpider4QuestionPushEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4QuestionProvider>();

        await provider.ExecuteAsync(new TouTiaoSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    [CapSubscribe(TouTiaoSpider4QuestionPullEto.EventNameString)]
    public Task HandleEventAsync(TouTiaoSpider4QuestionPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4QuestionProvider>();

        return provider.HandleEventAsync(eventData);
    }
}