// using Berry.Spider.TouTiao;
// using System.Threading.Tasks;
// using Volo.Abp.DependencyInjection;
// using Volo.Abp.EventBus.Distributed;
//
// namespace Berry.Spider.Consumers;
//
// /// <summary>
// /// 消费头条源数据推送消息
// /// </summary>
// public class TouTiaoSpiderPushEventHandler : IDistributedEventHandler<TouTiaoSpider4QuestionPushEto>, ITransientDependency
// {
//     private ITouTiaoSpiderAppService TouTiaoSpiderAppService { get; }
//
//     public TouTiaoSpiderPushEventHandler(ITouTiaoSpiderAppService service)
//     {
//         this.TouTiaoSpiderAppService = service;
//     }
//
//     public async Task HandleEventAsync(TouTiaoSpider4QuestionPushEto eventData)
//     {
//         await this.TouTiaoSpiderAppService.ExecuteAsync(new TouTiaoSpiderRequest
//         {
//             SourceFrom = eventData.SourceFrom,
//             Keyword = eventData.Keyword
//         });
//     }
// }