namespace Berry.Spider.Core;

public static class FileHelper
{
    /// <summary>
    /// 文件追加写入
    /// </summary>
    public static async Task WriteToFileAsync(string filePath, string content)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                await File.Create(filePath).DisposeAsync();
            }

            await using StreamWriter writer = new StreamWriter(filePath, append: true);
            await writer.WriteAsync(content);
        }
        catch (Exception ex)
        {
            // 错误处理，打印异常信息
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }
}