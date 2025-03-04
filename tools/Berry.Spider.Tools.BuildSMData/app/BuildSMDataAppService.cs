using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;
using Volo.Abp.TextTemplating;
using Volo.Abp.Uow;

namespace Berry.Spider.Tools.BuildSMData;

/// <summary>
/// 构建神马搜索数据
/// </summary>
public class BuildSMDataAppService : IBuildSMDataAppService
{
    private IServiceScopeFactory ServiceScopeFactory { get; }

    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private ITemplateRenderer TemplateRenderer { get; }
    private AbstractTemplateOptions AbstractTemplateOptions { get; }
    private ILogger<BuildSMDataAppService> Logger { get; }

    private readonly Regex _imgRegex = new Regex("<img.*></img>");

    public BuildSMDataAppService(IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider,
        ITemplateRenderer templateRenderer,
        IOptionsSnapshot<AbstractTemplateOptions> abstractTemplateOptions,
        ILogger<BuildSMDataAppService> logger)
    {
        this.ServiceScopeFactory = serviceScopeFactory;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<NormalTextAnalysisProvider>();
        this.TemplateRenderer = templateRenderer;
        this.AbstractTemplateOptions = abstractTemplateOptions.Value;
        this.Logger = logger;
    }

    [UnitOfWork]
    public virtual async Task RunAsync()
    {
        List<string> temp = new List<string>();
        IEnumerable<SpiderContent> contents = new List<SpiderContent>();

        using (var scop = this.ServiceScopeFactory.CreateScope())
        {
            //取出历史数据
            SpiderContentDapperRepository spiderContentDapperRepository =
                scop.ServiceProvider.GetRequiredService<SpiderContentDapperRepository>();

            contents = await spiderContentDapperRepository.GetAllAsync();
        }

        await Parallel.ForEachAsync(contents, new ParallelOptions
        {
            MaxDegreeOfParallelism = 20
        }, async (c, t) =>
        {
            //清除img标签
            c.Content = _imgRegex.Replace(c.Content, "");
            //拆散内容
            var list = await TextAnalysisProvider.InvokeAsync(c.Content);
            list.RandomSort();

            temp.AddRange(list);
        });

        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        //获取神马词库
        List<string> smWords = new List<string>();
        string[] smFiles = Directory.GetFiles(Path.Combine(filePath, "sm_words"));
        foreach (string file in smFiles)
        {
            string[] words = await File.ReadAllLinesAsync(file);
            smWords.AddRange(words);
        }

        //获取固定词
        List<string> fixedWords = new List<string>();
        string[] fixedFiles = Directory.GetFiles(Path.Combine(filePath, "fixed_words"));
        foreach (string file in fixedFiles)
        {
            string[] words = await File.ReadAllLinesAsync(file);
            fixedWords.AddRange(words);
        }

        //获取内容
        temp = temp.Where(c => !string.IsNullOrWhiteSpace(c)).OrderBy(c => Guid.CreateVersion7()).ToList();
        ListHelper listHelper = new ListHelper(temp, 50);

        //保存
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        for (int i = 0; i < smWords.Count - 1; i++)
        {
            StringBuilder builder = new StringBuilder();
            int currentIndex = 1;
            //神马词
            string smWord = smWords[i];
            //随机获取一个获取固定词
            Random fixedRandom = new Random(Guid.CreateVersion7().GetHashCode());
            string fixedWord = fixedWords[fixedRandom.Next(0, fixedWords.Count - 1)];

            //组装新的标题
            List<string> todoSaveItems = listHelper.GetList();
            if (todoSaveItems.Count == 0) break;

            string todoItem = "";
            while (string.IsNullOrEmpty(todoItem))
            {
                todoItem = todoSaveItems.OrderBy(c => Guid.CreateVersion7()).FirstOrDefault();
            }

            string remark = todoItem.Length >= 10
                ? todoItem.Substring(0, 10)
                : todoItem;
            string newTitle = $"{smWord}{fixedWord}({remark})";

            //主体内容
            foreach (var s in todoSaveItems)
            {
                builder.AppendLine($"<p>{currentIndex}、{s}</p>");
                currentIndex++;
            }

            //内容前加入加粗标题
            builder.Insert(0, $"<p><b>{smWord}{fixedWord}</b></p>");

            //组装摘要
            if (this.AbstractTemplateOptions.IsEnableAbstract)
            {
                //随机获取一个模版名称
                List<string> names = this.AbstractTemplateOptions.Templates.Select(c => c.Name).ToList();
                if (names.Count > 0)
                {
                    int index = new Random(Guid.CreateVersion7().GetHashCode()).Next(0, names.Count - 1);
                    string titleTemplateName = names[index];

                    //摘要
                    string text = await this.TemplateRenderer.RenderAsync(titleTemplateName, new
                    {
                        smWord = smWord,
                        fixedWord = fixedWord,
                        remark = string.Join(",", todoSaveItems.OrderBy(c => Guid.CreateVersion7()).Take(3))
                    });

                    builder.Insert(0, text);
                }
            }

            //组装数据
            var spiderContent = new SpiderContent(newTitle, builder.ToString(), SpiderSourceFrom.Text_File_Import);
            spiderContents.Add(spiderContent);
        }

        int pageSize = 100;
        int pageIndex = 0;
        int totalCount = spiderContents.Count;
        int pageCount = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
        while (pageIndex < pageCount)
        {
            var spiderContentsPage = spiderContents.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            try
            {
                using (var scop = this.ServiceScopeFactory.CreateScope())
                {
                    ISpiderContentRepository spiderRepository =
                        scop.ServiceProvider.GetRequiredService<ISpiderContentRepository>();
                    await spiderRepository.InsertManyAsync(spiderContentsPage, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            pageIndex++;
        }
    }
}