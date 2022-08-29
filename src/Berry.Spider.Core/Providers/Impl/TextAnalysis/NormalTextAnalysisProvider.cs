using System.Text.RegularExpressions;

namespace Berry.Spider.Core;

/// <summary>
/// 常规内容解析
/// <remarks>
///     参考数据：
///     <p>1、牵着手愿永久。</p><p>2、心意浓，雁南飞。</p></remarks>
/// </summary>
public class NormalTextAnalysisProvider : ITextAnalysisProvider
{
    private static readonly Regex ReplaceRegex = new Regex(@"^\d+(、|\.|\．|,|，)*");
    private static readonly Regex PTagRegex = new Regex(@"<p(?:(?!<\/p>).|\n)*?<\/p>");

    public Task<List<string>> InvokeAsync(string source)
    {
        List<string> list = new List<string>();

        if (PTagRegex.IsMatch(source))
        {
            var collection = PTagRegex.Matches(source);
            foreach (Match match in collection)
            {
                var item = match.Value;

                //替换<p>标签
                item = item.Replace("<p>", "").Replace("</p>", "");

                //通过正则替换的方式
                string newItem = ReplaceRegex.Replace(item, "");

                list.Add(newItem);
            }
        }

        return Task.FromResult(list);
    }
}