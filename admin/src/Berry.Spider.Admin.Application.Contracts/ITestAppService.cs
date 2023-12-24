using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Berry.Spider.Admin;

public interface ITestAppService : IApplicationService
{
    Task<int> GetAsync();
}