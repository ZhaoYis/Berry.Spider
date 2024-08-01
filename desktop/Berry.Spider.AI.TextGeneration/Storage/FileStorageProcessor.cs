using System.Collections.Concurrent;

namespace Berry.Spider.AI.TextGeneration.Storage;

public sealed class FileStorageProcessor
{
    private static readonly Lazy<FileStorageProcessor> _lazyHolder = new(() => new FileStorageProcessor());

    public static FileStorageProcessor Instance { get; } = _lazyHolder.Value;

    /// <summary>
    /// key：文件名
    /// value：文件路径
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> Cahe = new ConcurrentDictionary<string, string>();

    private FileStorageProcessor()
    {
    }

    public void Add(string fileName, string filePath)
    {
        Cahe.AddOrUpdate(fileName, k => filePath, (k, v) => v);
    }

    public string Remove(string fileName)
    {
        return Cahe.TryRemove(fileName, out string? filePath) ? filePath : string.Empty;
    }

    public string FirstOrDefault()
    {
        if (Cahe is not { Count: > 0 }) return string.Empty;
        KeyValuePair<string, string> firstOne = Cahe.FirstOrDefault();
        return Remove(firstOne.Key);
    }

    public IReadOnlyDictionary<string, string> GetAll() => Cahe;
}