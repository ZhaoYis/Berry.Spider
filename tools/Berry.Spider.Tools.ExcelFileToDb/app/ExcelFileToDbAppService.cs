using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace Berry.Spider.Tools.ExcelFileToDb;

/// <summary>
/// excel文件数据导入到MySQL
/// </summary>
public class ExcelFileToDbAppService : IExcelFileToDbAppService
{
    private ISpiderContentRepository SpiderRepository { get; }
    private ILogger<ExcelFileToDbAppService> Logger { get; }

    public ExcelFileToDbAppService(ISpiderContentRepository spiderRepository, ILogger<ExcelFileToDbAppService> logger)
    {
        SpiderRepository = spiderRepository;
        Logger = logger;
    }

    public virtual async Task RunAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.xlsx", SearchOption.AllDirectories);
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        foreach (string file in files)
        {
            DataTable? dt = OfficeHelper.ReadFromExcel(file, isFirstRowColumn: true);
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //组装数据
                    string title = dt.Rows[i][0].ToString();
                    string content = dt.Rows[i][1].ToString();
                    string keywords = dt.Rows[i][2].ToString();

                    if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                    {
                        var spiderContent = new SpiderContent(title, content, SpiderSourceFrom.Excel_File_Import, keywords: keywords, tag: keywords);
                        spiderContents.Add(spiderContent);
                    }
                }
            }
        }

        if (spiderContents.Count == 0) return;

        // //清理相似度比较高的记录
        // List<SpiderContent> currentList = JsonConvert.DeserializeObject<List<SpiderContent>>(JsonConvert.SerializeObject(spiderContents));
        // foreach (SpiderContent content in spiderContents)
        // {
        //     for (int i = currentList.Count - 1; i >= 0; i--)
        //     {
        //         SpiderContent current = currentList[i];
        //
        //         var sim = StringHelper.Sim(content.Title, current.Title);
        //         if (sim >= 0.8 && sim - 1 != 0)
        //         {
        //             Console.WriteLine($"标题1：{content.Title}，标题2：{current.Title}，相似度：{sim * 100:F}%");
        //             currentList.RemoveAt(i);
        //         }
        //     }
        // }

        int pageSize = 100;
        int pageIndex = 0;
        int totalCount = spiderContents.Count;
        int pageCount = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
        while (pageIndex < pageCount)
        {
            var spiderContentsPage = spiderContents.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            try
            {
                await this.SpiderRepository.InsertManyAsync(spiderContentsPage, true);
                await Task.Delay(2).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            pageIndex++;
        }
    }

    public virtual Task ExportToExcelAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.xlsx", SearchOption.AllDirectories);
        List<ExcelDataResource> worksheets = new List<ExcelDataResource>();
        foreach (string file in files)
        {
            DataTable? dt = OfficeHelper.ReadFromExcel(file, isFirstRowColumn: true);
            if (dt != null)
            {
                List<ExcelDataRow> rows = new List<ExcelDataRow>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //组装数据
                    string? title = dt.Rows[i][1].ToString();
                    string? content = dt.Rows[i][2].ToString();

                    if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                    {
                        title = title.Split("-").First().Replace("组词", "").Replace("的意思", "");
                        title = $"{title}的意思及解释-{title}的拼音读音";
                        rows.Add(new ExcelDataRow
                        {
                            Title = title,
                            Content = content
                        });
                    }
                }

                worksheets.Add(new ExcelDataResource
                {
                    SheetName = "Sheet1",
                    RowNum = 1,
                    Rows = rows
                });
            }
        }

        OfficeHelper.WriteToExcel(worksheets, Path.Combine(filePath, $"{DateTime.Now:yyyyMMddHHmmssfff}.xlsx"));
        return Task.CompletedTask;
    }

    public async Task CleanAndExportToExcelAsync()
    {
        string filePath = Path.Combine("/");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.php", SearchOption.AllDirectories);
        List<ExcelDataResource> worksheets = new List<ExcelDataResource>();
        List<ExcelDataRow> rows = new List<ExcelDataRow>(100_00);
        HtmlDocument doc = new HtmlDocument();
        foreach (string file in files)
        {
            doc.Load(file);
            HtmlNode rootNode = doc.DocumentNode;

            if (rootNode.InnerText.Contains("的拼音,读音,繁体,注音,火星文,平调拼音"))
            {
                // 删除所有 <audio> 标签
                foreach (var audioNode in rootNode.SelectNodes("//audio") ?? new HtmlNodeCollection(null))
                {
                    audioNode.Remove();
                }

                // 删除所有 <img> 标签
                foreach (var imgNode in rootNode.SelectNodes("//img") ?? new HtmlNodeCollection(null))
                {
                    imgNode.Remove();
                }

                // 删除最后一个 <p> 标签
                var pNodes = rootNode.LastChild;
                pNodes.Remove();

                //标题
                string? title = rootNode.FirstChild.InnerText;
                title = title.Replace("的拼音,读音,繁体,注音,火星文,平调拼音", "");
                title = $"{title}的拼音读音-{title}的意思及解释";
                rootNode.FirstChild.Remove();
                //内容
                var newPNode = HtmlNode.CreateNode($"<p>{title}</p>");
                rootNode.PrependChild(newPNode);
                string? content = rootNode.OuterHtml;

                if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                {
                    rows.Add(new ExcelDataRow
                    {
                        Title = title,
                        Content = content
                    });
                }

                Console.WriteLine(@"标题：[{0}]处理成功", title);
            }

            if (rows.Count == 100_00)
            {
                worksheets.Add(new ExcelDataResource
                {
                    SheetName = "Sheet1",
                    RowNum = 1,
                    Rows = rows
                });
                OfficeHelper.WriteToExcel(worksheets, Path.Combine(filePath, $"{DateTime.Now:yyyyMMddHHmmssfff}.xlsx"));
                rows.Clear();
                worksheets.Clear();
            }
        }
    }
}