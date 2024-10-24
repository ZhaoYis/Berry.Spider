﻿using Berry.Spider.Proxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Berry.Spider.Tests;

public class HttpProxy_UnitTest
{
    [Fact]
    public async Task GetProxyUri_ShouldReturnResult_WhenRequestSuccessful()
    {
        //arrange
        var options = new Mock<IOptionsSnapshot<HttpProxyOptions>>();
        options.Setup(c => c.Value).Returns(new HttpProxyOptions { ProxyPoolApiHost = "http://124.223.62.114:5010" });

        var messageHandler = new Mock<HttpMessageHandler>();
        messageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    "{'anonymous':'','check_count':1321,'fail_count':0,'https':false,'last_status':true,'last_time':'2022-03-25 13:38:08','proxy':'222.249.173.24:84','region':'','source':'freeProxy10'}")
            });

        var httpClientFactory = new Mock<IHttpClientFactory>();
        var httpClient = new HttpClient(messageHandler.Object);
        httpClientFactory.Setup(c => c.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var proxyPoolHttpClient = new ProxyPoolHttpClient(options.Object, httpClientFactory.Object.CreateClient());
        var logger = new Mock<ILogger<DefaultHttpProxy>>();

        var defaultProxy = new DefaultHttpProxy(logger.Object, proxyPoolHttpClient, options.Object);

        //act
        var uri = await defaultProxy.GetProxyUriAsync();

        //assert
        uri.ShouldNotBeNullOrEmpty();
    }
}