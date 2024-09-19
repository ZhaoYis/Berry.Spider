using Berry.Spider.Core;
using Xunit;
using Xunit.Abstractions;

namespace Berry.Spider.Tests;

public class StringHelper_UnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public StringHelper_UnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("我来到北京清华大学", "清华大学是中国的最高学府")]
    private void CalculateCosineSimilarity_Test(string text1, string text2)
    {
        double d1 = StringHelper.Sim(text1, text2);
        _testOutputHelper.WriteLine($"相似度1：{d1}");

        double d2 = StringHelper.CalculateCosineSimilarity(text1, text2);
        _testOutputHelper.WriteLine($"相似度2：{d2}");
    }
}