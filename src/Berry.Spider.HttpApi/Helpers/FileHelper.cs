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
            await using (var stream = System.IO.File.Create(filePath))
            {
                await _file.CopyToAsync(stream);
            }

            if (System.IO.File.Exists(filePath))
            {
                List<string> rows = (await System.IO.File.ReadAllLinesAsync(filePath))
                    .Where(c => !string.IsNullOrWhiteSpace(c.Trim()))
                    .Distinct()
                    .ToList();
                if (rows.Count > 0)
                {
                    foreach (string row in rows)
                    {
                        await _onInvoke.Invoke(row);
                    }
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
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}