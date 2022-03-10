using System.Text.RegularExpressions;

namespace Berry.Spider.Core;

public class TextAnalysisProvider : ITextAnalysisProvider
{
    //https://www.jianshu.com/p/a1a9a98c7bd9
    //https://blog.csdn.net/xjp_xujiping/article/details/50210533
    
    private static readonly Regex ReplaceRegex = new Regex(@"^\d+(、|\.|\．|,|，)*");

    public async Task<List<string>> InvokeAsync(string source)
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
                
                //清理掉所有空格
                string data = item.Replace(" ", "");

                //通过正则替换的方式
                string newItem = ReplaceRegex.Replace(data, "");
                if (!string.IsNullOrEmpty(newItem))
                {
                    //处理黑名单词汇，这部分词语直接替换
                    newItem = newItem.ReplaceTo(Convert.ToChar("")).Trim();

                    list.Add(newItem);
                }
            }
        }

        return await Task.FromResult(list);
    }
}