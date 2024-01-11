using Microsoft.AspNetCore.Http;

namespace Berry.Spider;

public class FileHelper
{
    private readonly IFormFile _file;
    private readonly Func<string, Task> _onInvoke;

    public FileHelper(IFormFile file, Func<string, Task> onInvoke)
    {
        _file = file;
        _onInvoke = onInvoke;
    }

    public async Task InvokeAsync()
    {
        var filePath = Path.GetTempFileName();

        try
        {
            await using (var stream = File.Create(filePath))
            {
                await _file.CopyToAsync(stream);
            }
            await foreach (string row in this.TryReadLinesAsync(filePath))
            {
                if (!string.IsNullOrWhiteSpace(row.Trim()))
                {
                    await _onInvoke.Invoke(row.Trim());
                    await Task.Delay(1000);
                }
            }
        }
        catch (Exception e)
        {
            //ignore..
        }
        finally
        {
            //删除临时文件
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private IAsyncEnumerable<string> TryReadLinesAsync(string filePath)
    {
        return File.ReadLinesAsync(filePath);
    }
}