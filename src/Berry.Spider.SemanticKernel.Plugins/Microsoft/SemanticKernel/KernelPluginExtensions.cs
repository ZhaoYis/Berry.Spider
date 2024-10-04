using Berry.Spider.SemanticKernel.Plugins;

namespace Microsoft.SemanticKernel;

public static class KernelPluginExtensions
{
    /// <summary>
    /// 注入自定义插件
    /// </summary>
    public static void AddPlugins(this IKernelBuilderPlugins plugins)
    {
        plugins.AddFromType<DateTimePlugin>("date_time_plugin");

        // plugins.AddFromFunctions("date_time_plugin",
        // [
        //     KernelFunctionFactory.CreateFromMethod(
        //         method: () => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //         functionName: "get_time",
        //         description: "Get the current time"
        //     ),
        //     KernelFunctionFactory.CreateFromMethod(
        //         method: (DateTime start, DateTime end) => (end - start).TotalSeconds,
        //         functionName: "diff_time",
        //         description: "Get the difference between two times in seconds"
        //     )
        // ]);
    }
}