using Berry.Spider.Core;
using Shouldly;
using Xunit;

namespace Berry.Spider.Tests;

public class StringExtensions_UnitTest
{
    [Fact]
    public void Replace_Can_Be_Successful_Test()
    {
        string source = "这是一段文字條萊垍頭";

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
}