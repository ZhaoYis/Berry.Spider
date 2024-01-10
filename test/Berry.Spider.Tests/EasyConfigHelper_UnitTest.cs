using Berry.Spider.Core;
using Shouldly;
using Xunit;

namespace Berry.Spider.Tests;

public class EasyConfigHelper_UnitTest
{
    [Fact]
    public void Set_Get_Test()
    {
        EasyConfigHelper helper = new EasyConfigHelper("123.ini");
        helper.Set("A", "B", "C");

        string value = helper.Get("A", "B");
        value.ShouldBe("C");
    }
}