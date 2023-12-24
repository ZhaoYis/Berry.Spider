using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider.Admin;

[Area(AdminGlobalConstants.ModelName)]
[RemoteService(Name = AdminGlobalConstants.RemoteServiceName)]
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