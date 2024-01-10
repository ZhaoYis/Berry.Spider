using Microsoft.Extensions.Configuration;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.Util;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ServDetector;

public class QiniuDownloadManager : ITransientDependency
{
    private QiniuOptions? QiniuOptions { get; }

    public QiniuDownloadManager(IConfiguration configuration)
    {
        this.QiniuOptions = configuration.GetSection(nameof(QiniuOptions)).Get<QiniuOptions>();
    }

    /// <summary>
    /// 下载私有空间中的文件
    /// </summary>
    public async Task DownloadPrivateFileAsync(string rawUrl, string saveFilePath)
    {
        string downloadSourceUrl = $"{this.QiniuOptions?.DownloadBasicUrl}/{rawUrl}";
        Mac mac = new Mac(this.QiniuOptions?.AccessKey, this.QiniuOptions?.SecretKey);
        string accUrl = DownloadManager.CreateSignedUrl(mac, downloadSourceUrl);

        HttpResult result = await DownloadManager.DownloadAsync(accUrl, saveFilePath);
    }
}