using SharpCompress.Archives;
using SharpCompress.Common;

namespace Berry.Spider.Core;

public static class ZipHelper
{
    /// <summary>
    /// 解压缩一个 zip 文件。
    /// </summary>
    /// <param name="zipedFile">压缩文件</param>
    /// <param name="rootDirectory">解压目录</param>
    public static string UnZip(string zipFile, string rootDirectory)
    {
        if (string.IsNullOrEmpty(rootDirectory))
        {
            rootDirectory = Path.GetDirectoryName(zipFile) ?? "";
        }

        string zipFilePath = Directory.CreateDirectory(rootDirectory).FullName;
        ArchiveFactory.WriteToDirectory(zipFile, zipFilePath, new ExtractionOptions
        {
            ExtractFullPath = true,
            Overwrite = true
        });

        return zipFilePath;
    }
}