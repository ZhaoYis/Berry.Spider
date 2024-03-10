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

            using (StreamReader reader = new StreamReader(filePath))
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                string? row = (await reader.ReadLineAsync())?.Trim();
                while (!string.IsNullOrEmpty(row))
                {
                    await _onInvoke.Invoke(row).ConfigureAwait(false);
                    await Task.Delay(random.Next(1000, 1500)).ConfigureAwait(false);
                    row = (await reader.ReadLineAsync())?.Trim();
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
}