using System.Collections.Concurrent;

namespace Berry.Spider.AI.TextGeneration.Storage;

public static class FileStorageProcessor
{
    /// <summary>
    /// key：文件名
    /// value：文件路径
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> Cahe = new ConcurrentDictionary<string, string>();

    public static void Add(string fileName, string filePath)
    {
        Cahe.AddOrUpdate(fileName, k => filePath, (k, v) => v);
    }

    public static string Remove(string fileName)
    {
        return Cahe.TryRemove(fileName, out string? filePath) ? filePath : string.Empty;
    }

    public static string GetOne()
    {
        if (Cahe is not { Count: > 0 }) return string.Empty;
        KeyValuePair<string, string> firstOne = Cahe.FirstOrDefault();
        return Remove(firstOne.Key);
    }
}