namespace Berry.Spider.Mmonly.Contracts;

public record MmonlyFileDownloadArgs
{
    /// <summary>
    /// 待下载地址
    /// </summary>
    public string TodoDownloadUrl { get; init; }
}