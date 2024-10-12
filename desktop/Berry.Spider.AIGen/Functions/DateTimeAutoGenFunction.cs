using System;
using System.Threading.Tasks;
using AutoGen.Core;

namespace Berry.Spider.AIGen;

public partial class DateTimeAutoGenFunction
{
    /// <summary>
    /// 获取当前系统时间
    /// </summary>
    /// <returns>当前系统时间</returns>
    [Function("GetCurrentDateTime", "获取当前系统时间")]
    public Task<string> GetCurrentDateTimeAsync()
    {
        return Task.FromResult(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}