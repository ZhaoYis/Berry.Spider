using System.Text.RegularExpressions;
using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

public class SogouSpider4WenWenTextAnalysisProvider : ITextAnalysisProvider
{
    private static readonly Regex ReplaceRegex = new Regex(@"^\d+(、|\.|\．|,|，)*");

    public Task<List<string>> InvokeAsync(string source)
    {
        List<string> list = new List<string>();

        //根据\n进行分割
        string[] sources = source.Split('\n');
        if (sources.Length > 0)
        {
            //验证是否有序号，例如：[1、]、[1.]、[一、]等
            foreach (string item in sources)
            {
                //通过正则替换的方式
                string newItem = ReplaceRegex.Replace(item, "");
                //处理黑名单词汇，这部分词语直接替换
                newItem = newItem.ReplaceTo(' ').Replace(" ", "").Trim();
                list.Add(newItem);
            }
        }

        return Task.FromResult(list);
    }
}