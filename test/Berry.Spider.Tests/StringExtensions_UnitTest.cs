using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Berry.Spider.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Berry.Spider.Tests;

public class StringExtensions_UnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public StringExtensions_UnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Replace_Can_Be_Successful_Test()
    {
        string source = "这是一段文字招聘兼职";

        string result = source.ReplaceTo('*');

        result.ShouldBe("这是一段文字****");
    }

    [Fact]
    public void Replace_Source_Null_Test()
    {
        string source = "";
        string result = source.ReplaceTo('*');
        result.ShouldBe("");
    }

    [Fact]
    public async Task Resolve_TouTiao_Url_Test()
    {
        string source =
            "/search/jump?url=https%3A%2F%2Ftsearch.toutiaoapi.com%2Fs%2Fsearch_wenda%2Flist%3Fenable_miaozhen_page%3D1%26enter_answer_id%3D7175162421706048012%26enter_from%3Dsearch_result%26outer_show_aid%3D7175162421706048012%26qid%3D6931623923998900488%26relate_type%3D0%26search_id%3D202311052200222B7841AC659CB8F8D369&aid=4916&jtoken=b39b0592532f285e70b939e6a876efe8508d3d85428a00d33a8f5466d0bbd1f13182fe4b2dacce582ea0a848d8dd5018242e41a8ca8c4db963a7961d3ea0b8975bb1b02524eeb0e8cb4450ea05408cd2";
        string result = await ResolveAsync(source);
        result.ShouldStartWith("https://");
    }

    private Task<string> ResolveAsync(string sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl)) return Task.FromResult("");

        sourceUrl = HttpUtility.UrlDecode(sourceUrl);
        
        string pattern = @"\/search\/jump\?url=(.*)";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(sourceUrl);
        if (match.Success)
        {
            string url = match.Groups[1].Value;
            return Task.FromResult<string>(url);
        }

        return Task.FromResult("");
    }
}