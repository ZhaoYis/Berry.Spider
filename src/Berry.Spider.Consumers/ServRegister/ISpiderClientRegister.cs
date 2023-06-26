using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Consumers;

public interface ISpiderClientRegister : ITransientDependency
{
    /// <summary>
    /// 服务注册
    /// </summary>
    /// <returns></returns>
    Task RegisterAsync();

    /// <summary>
    /// 取消服务注册
    /// </summary>
    /// <returns></returns>
    Task UnRegisterAsync();
}