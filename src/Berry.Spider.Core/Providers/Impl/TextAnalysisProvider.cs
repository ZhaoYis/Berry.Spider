using System.Text;

namespace Berry.Spider.Core;

public class TextAnalysisProvider : ITextAnalysisProvider
{
    //https://www.jianshu.com/p/a1a9a98c7bd9
    //https://blog.csdn.net/xjp_xujiping/article/details/50210533
    
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
                char[] array = item.ToCharArray();
                List<char> newArray = new List<char>();
                foreach (char c in array)
                {
                    //26个字母、数字、符号ASCII码
                    if (c <= 127) continue;
                    //TODO:中文符号

                    newArray.Add(c);
                }

                string newItem = Encoding.UTF8.GetString(Encoding.Default.GetBytes(newArray.ToArray()));
                list.Add(newItem);
            }
        }

        return await Task.FromResult(list);
    }
}