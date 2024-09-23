using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Berry.Spider.AIGen
{
    public class OpenAIHttpClientHandler : HttpClientHandler
    {
        private readonly string _openAiBaseAddress;

        public OpenAIHttpClientHandler(string openAiBaseAddress)
        {
            _openAiBaseAddress = openAiBaseAddress;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            UriBuilder uriBuilder;
            Uri uri = new Uri(_openAiBaseAddress);
            string scheme = uri.Scheme;
            string host = uri.Host;
            int port = uri.Port;

            switch (request.RequestUri?.LocalPath)
            {
                case "/v1/chat/completions" or "/chat/completions":
                    uriBuilder = new UriBuilder(request.RequestUri)
                    {
                        Scheme = scheme,
                        Host = host,
                        Port = port,
                        Path = "v1/chat/completions"
                    };
                    request.RequestUri = uriBuilder.Uri;
                    break;
                case "/v1/embeddings" or "/embeddings":
                    uriBuilder = new UriBuilder(request.RequestUri)
                    {
                        Scheme = scheme,
                        Host = host,
                        Port = port,
                        Path = "v1/embeddings"
                    };
                    request.RequestUri = uriBuilder.Uri;
                    break;
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}