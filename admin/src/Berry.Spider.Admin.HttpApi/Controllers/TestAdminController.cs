using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider.Admin;

[Area(GlobalConstants.ModelName)]
[RemoteService(Name = GlobalConstants.RemoteServiceName)]
public class TestAdminController : AdminController, ITestAppService
{
    private ITestAppService _testAppService;

    public TestAdminController(ITestAppService testAppService)
    {
        _testAppService = testAppService;
    }

    public Task<int> GetAsync()
    {
        return _testAppService.GetAsync();
    }
}