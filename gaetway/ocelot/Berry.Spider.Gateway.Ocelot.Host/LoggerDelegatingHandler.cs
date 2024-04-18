using System.Net.Http.Headers;
using System.Text;

namespace Berry.Spider.Gateway.Ocelot.Host;

public class LoggerDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggerDelegatingHandler> Logger;

    public LoggerDelegatingHandler(ILogger<LoggerDelegatingHandler> logger)
    {
        this.Logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpRequestHeaders headers = request.Headers;

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        StringBuilder builder = new StringBuilder();
        foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
        {
            builder.AppendLine(header.Key + " --> " + string.Join("，", header.Value) + "</br>");
        }

        //请求服务地址
        builder.AppendLine("RequestUri --> " + request.RequestUri.ToString() + "</br>");

        // //请求消息
        // string content = await request.Content.ReadAsStringAsync();
        // builder.AppendLine("请求消息 --> " + content + "</br>");
        //
        // //响应消息
        // string result = await response.Content.ReadAsStringAsync();
        // builder.AppendLine("响应消息 --> " + result + "</br>");

        this.Logger.LogInformation(builder.ToString());

        return response;
    }
}