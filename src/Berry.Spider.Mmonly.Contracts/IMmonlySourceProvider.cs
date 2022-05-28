namespace Berry.Spider.Mmonly.Contracts;

public interface IMmonlySourceProvider
{
    IEnumerable<string> GetUrls();
}