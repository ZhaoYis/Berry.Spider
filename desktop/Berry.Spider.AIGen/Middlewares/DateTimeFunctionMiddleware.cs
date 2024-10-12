using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoGen.Core;
using AutoGen.SemanticKernel;

namespace Berry.Spider.AIGen;

public static class DateTimeFunctionMiddleware
{
    /// <summary>
    /// 注册自定义函数
    /// </summary>
    public static void RegisterFunctions(this SemanticKernelAgent agent)
    {
        var dateTimeFunction = new DateTimeAutoGenFunction();
        var functionCallMiddleware = new FunctionCallMiddleware(functions: new List<FunctionContract>
        {
            dateTimeFunction.GetCurrentDateTimeAsyncFunctionContract
        }, functionMap: new Dictionary<string, Func<string, Task<string>>>
        {
            {
                dateTimeFunction.GetCurrentDateTimeAsyncFunctionContract.Name, dateTimeFunction.GetCurrentDateTimeAsyncWrapper
            }
        });

        //agent.RegisterMiddleware(functionCallMiddleware);
        agent.RegisterStreamingMiddleware(functionCallMiddleware);
    }
}