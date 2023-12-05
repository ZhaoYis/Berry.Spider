using System.Text.RegularExpressions;
using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

public class SougouResolveJumpUrlProvider : IResolveJumpUrlProvider
{
    private static Regex Sougou_Com_Regex = new(@"sogou\.com");
    private static Regex Url_Regex = new(@"^(https?|ftp):\/\/[^\s\/$.?#].[^\s]*$");

    public Task<string> ResolveAsync(string sourceUrl)
    {
        if (!Url_Regex.Match(sourceUrl).Success)
        {
            return Task.FromResult("");
        }

        if (Sougou_Com_Regex.Match(sourceUrl).Success)
        {
            return Task.FromResult(sourceUrl);
        }

        return Task.FromResult("");
    }
}