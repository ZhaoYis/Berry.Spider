using Berry.Spider.Domain.Shared;

namespace Berry.Spider;

public interface ISpiderEto
{
    SpiderSourceFrom SourceFrom { get; set; }
}