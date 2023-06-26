using System.Net.Http.Json;

namespace Berry.Spider.Core;

public class UserAgentHttpClient
{
    private readonly HttpClient _httpClient;

    public UserAgentHttpClient(HttpClient client)
    {
        client.BaseAddress = new Uri("https://www.useragents.me");
        _httpClient = client;
    }

    public async Task<List<string>?> GetUserAgentsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<UserAgentMeResult>("/api");

        if (response?.data is {Count: > 0})
        {
            var result = response.data.Select(ua => ua.ua).ToList();
            return result;
        }

        return default;
    }
}

internal class UserAgentMeResult
{
    public string about { get; set; }

    public string terms { get; set; }

    public List<UserAgentMeData> data { get; set; }

    public long updated { get; set; }

    public string thanks { get; set; }
}

internal class UserAgentMeData
{
    public string ua { get; set; }

    public decimal pct { get; set; }
}