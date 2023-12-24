using System.Threading.Tasks;

namespace Berry.Spider.Admin;

public class TestAppService : AdminAppService, ITestAppService
{
    public Task<int> GetAsync()
    {
        return Task.FromResult(42);
    }
}