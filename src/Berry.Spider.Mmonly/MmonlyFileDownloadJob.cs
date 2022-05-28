using System.Text.RegularExpressions;
using Berry.Spider.Core;
using Berry.Spider.Mmonly.Contracts;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Berry.Spider.Mmonly;

public class MmonlyFileDownloadJob : AsyncBackgroundJob<MmonlyFileDownloadArgs>, ITransientDependency
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IGuidGenerator GuidGenerator { get; }

    public MmonlyFileDownloadJob(IWebElementLoadProvider webElementLoadProvider, IGuidGenerator guidGenerator)
    {
        WebElementLoadProvider = webElementLoadProvider;
        GuidGenerator = guidGenerator;
    }

    /// <summary>
    /// 这里执行具体的下载任务
    /// </summary>
    public override async Task ExecuteAsync(MmonlyFileDownloadArgs args)
    {
        int totalPage = await this.GetPageCountAsync(args.TodoDownloadUrl);
        if (totalPage > 0)
        {
            //https://www.mmonly.cc/mmtp/nymn/340905_5.html
            //第一页不需要拼接页码
            Regex regex = new Regex(@"\d+");
            for (int i = 1; i <= totalPage; i++)
            {
                if (i == 1)
                {
                    await this.DownloadFileAsync(args.TodoDownloadUrl);
                }
                else
                {
                    string id = regex.Match(args.TodoDownloadUrl).Value;
                    if (!string.IsNullOrEmpty(id))
                    {
                        id = $"{id}_{i}";
                        string pageUrl = regex.Replace(args.TodoDownloadUrl, id);
                        await this.DownloadFileAsync(pageUrl);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    private async Task DownloadFileAsync(string pageUrl)
    {
        try
        {
            await this.WebElementLoadProvider.InvokeAsync(
                pageUrl,
                drv =>
                {
                    try
                    {
                        return drv.FindElement(By.Id("big-pic"));
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                },
                async root =>
                {
                    if (root == null) return;

                    var resultContent = root.FindElement(By.TagName("img"));
                    if (resultContent != null)
                    {
                        string src = resultContent.GetAttribute("src");
                        if (!string.IsNullOrEmpty(src))
                        {
                            var basePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "download");
                            if (!Directory.Exists(basePath))
                            {
                                Directory.CreateDirectory(basePath);
                            }

                            //从url中获取文件扩展名
                            string fileExtension = Path.GetExtension(src);
                            if(fileExtension.StartsWith("."))
                            {
                                fileExtension = fileExtension.Substring(1);
                            }
                            
                            string fileName = GuidGenerator.Create().ToString("N");
                            string filePath = $"{basePath}/{fileName}.{fileExtension}";
                            using (var client = new System.Net.WebClient())
                            {
                                await client.DownloadFileTaskAsync(src, filePath);
                            }
                        }
                    }
                });
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }
    }

    private async Task<int> GetPageCountAsync(string url)
    {
        try
        {
            int result = await this.WebElementLoadProvider.InvokeAsync<int>(
                url,
                drv =>
                {
                    try
                    {
                        return drv.FindElement(By.ClassName("pages"));
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }, root =>
                {
                    if (root == null) return Task.FromResult(0);

                    var resultContent = root.FindElements(By.TagName("a"));
                    if (resultContent.Count > 0)
                    {
                        foreach (IWebElement element in resultContent)
                        {
                            string content = element.Text;
                            Regex regex = new Regex(@"\d+");
                            if (regex.IsMatch(content))
                            {
                                int pageCount = int.Parse(regex.Match(content).Value);
                                return Task.FromResult<int>(pageCount);
                            }
                        }
                    }

                    return Task.FromResult<int>(0);
                });

            return result;
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }

        return 0;
    }
}