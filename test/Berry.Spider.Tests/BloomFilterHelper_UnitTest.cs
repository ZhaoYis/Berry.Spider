using Berry.Spider.Core;
using System.Threading.Tasks;
using Xunit;

namespace Berry.Spider.Tests;

public class BloomFilterHelper_UnitTest
{
    [Fact]
    public async Task Add()
    {
        BloomFilterHelper<string> bloomFilterHelper = new BloomFilterHelper<string>(99);
        bloomFilterHelper.Add("大师兄");
        bloomFilterHelper.Add("大哥大");

        double truthiness = bloomFilterHelper.Truthiness;

        Assert.True(bloomFilterHelper.Contains("大师兄"));
    }
}