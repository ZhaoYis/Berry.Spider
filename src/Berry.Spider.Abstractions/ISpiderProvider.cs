namespace Berry.Spider.Abstractions;

public interface ISpiderProvider
{
    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    Task PushAsync<T>(T push) where T : class, ISpiderPushEto;

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    /// <returns></returns>
    Task ExecuteAsync<T>(T request) where T : class, ISpiderRequest;

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    Task HandleEventAsync<T>(T eventData) where T : class, ISpiderPullEto;
}