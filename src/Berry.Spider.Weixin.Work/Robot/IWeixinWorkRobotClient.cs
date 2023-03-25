using Refit;

namespace Berry.Spider.Weixin.Work;

public interface IWeixinWorkRobotClient
{
    /// <summary>
    /// 通过机器人向企业微信群发送消息
    /// </summary>
    /// <returns></returns>
    [Post("/cgi-bin/webhook/send?key={app_key}")]
    Task<WeixinResult> SendAsync(string app_key, IWeixinRobotMessage request);
}