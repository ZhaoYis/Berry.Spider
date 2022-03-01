using System.Text.Json.Serialization;

namespace Berry.Spider.Proxy;

public class ProxyPoolResult
{
    [JsonPropertyName("https")]
    public bool Https { get; set; }
    
    [JsonPropertyName("last_status")]
    public bool LastStatus { get; set; }
    
    [JsonPropertyName("proxy")]
    public string Proxy { get; set; }
}