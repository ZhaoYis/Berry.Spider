using Volo.Abp.Caching;

namespace Berry.Spider.Core;

[CacheName("UserAgents")]
public class UserAgentCacheItem
{
    public List<string> UaPools { get; set; }
}