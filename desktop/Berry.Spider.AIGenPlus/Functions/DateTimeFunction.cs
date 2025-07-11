using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Functions;

public class DateTimeFunction : AIFunction, ITransientDependency
{
    /// <summary>
    /// 获取当前系统时间
    /// </summary>
    /// <returns></returns>
    protected override Task<object?> InvokeCoreAsync(IEnumerable<KeyValuePair<string, object?>> arguments, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Invoke the system function get_current_time()...");
        return Task.FromResult<object?>(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}