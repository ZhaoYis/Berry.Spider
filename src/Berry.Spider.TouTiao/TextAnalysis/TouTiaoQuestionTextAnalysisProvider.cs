using Berry.Spider.Core;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条问答内容解析器
/// </summary>
public class TouTiaoQuestionTextAnalysisProvider : ITextAnalysisProvider
{
    //https://www.jianshu.com/p/a1a9a98c7bd9
    //https://blog.csdn.net/xjp_xujiping/article/details/50210533

    private static readonly Regex ReplaceRegex = new Regex(@"^\d+(、|\.|\．|,|，)*");
    private SpiderOptions Options { get; }

    public TouTiaoQuestionTextAnalysisProvider(IOptionsSnapshot<SpiderOptions> options)
    {
        this.Options = options.Value;
    }

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
                #region 通过ASCII码替换

                // char[] array = item.ToCharArray();
                // List<char> newArray = new List<char>();
                // foreach (char c in array)
                // {
                //     //26个字母、数字、符号ASCII码
                //     if (c <= 127) continue;
                //     //中文符号
                //     if (c.IsChineseSymbol()) continue;
                //
                //     newArray.Add(c);
                // }
                // string newItem = Encoding.UTF8.GetString(Encoding.Default.GetBytes(newArray.ToArray()));

                #endregion

                //通过正则替换的方式
                string newItem = ReplaceRegex.Replace(item, "");
                //处理黑名单词汇，这部分词语直接替换
                newItem = newItem.ReplaceTo(' ').Replace(" ", "").Trim();

                //验证内容最小长度
                if (!string.IsNullOrWhiteSpace(newItem) && newItem.Length > this.Options.MinContentLength)
                {
                    list.Add(newItem);
                }
            }
        }

        return Task.FromResult(list);
    }
}