// using Berry.Spider.Baidu;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Threading.Tasks;
// using Volo.Abp.DependencyInjection;
// using Volo.Abp.EventBus.Distributed;
//
// namespace Berry.Spider.Consumers;
//
// /// <summary>
// /// 百度：相关推荐
// /// </summary>
// public class BaiduSpider4RelatedSearchEventHandler : IDistributedEventHandler<BaiduSpider4RelatedSearchPullEto>,
//     ITransientDependency
// {
//     private ILogger<BaiduSpider4RelatedSearchEventHandler> Logger { get; }
//     private IBaiduSpiderAppService BaiduSpiderAppService { get; }
//
//     public BaiduSpider4RelatedSearchEventHandler(ILogger<BaiduSpider4RelatedSearchEventHandler> logger,
//         IBaiduSpiderAppService service)
//     {
//         this.Logger = logger;
//         this.BaiduSpiderAppService = service;
//     }
//
//     public async Task HandleEventAsync(BaiduSpider4RelatedSearchPullEto eventData)
//     {
//         try
//         {
//             await this.BaiduSpiderAppService.HandleEventAsync<BaiduSpider4RelatedSearchPullEto>(eventData);
//         }
//         catch (Exception exception)
//         {
//             this.Logger.LogException(exception);
//         }
//         finally
//         {
//             //ignore..
//         }
//     }
// }