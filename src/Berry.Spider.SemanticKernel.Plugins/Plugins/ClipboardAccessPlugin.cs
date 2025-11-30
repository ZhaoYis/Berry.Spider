using System.ComponentModel;
using System.Diagnostics;
using Microsoft.SemanticKernel;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.SemanticKernel.Plugins;

public class ClipboardAccessPlugin : ITransientDependency
{
    /// <summary>
    /// 复制文本到剪贴板
    /// </summary>
    /// <param name="content"></param>
    /// <exception cref="PlatformNotSupportedException"></exception>
    [KernelFunction("set_clipboard")]
    [Description("Copies the provided content to the clipboard.")]
    public static void SetClipboard(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        string? fileName;
        if (OperatingSystem.IsWindows())
        {
            // Windows
            fileName = "clip";
        }
        else if (OperatingSystem.IsMacOS())
        {
            // macOS
            fileName = "pbcopy";
        }
        else if (OperatingSystem.IsLinux())
        {
            // Linux (需要 xclip 或 xsel)
            fileName = "xclip"; // or "xsel"
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported OS.");
        }

        using Process process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardInput = true,
            UseShellExecute = false
        };

        process.Start();
        process.StandardInput.Write(content);
        process.StandardInput.Close();
    }
}