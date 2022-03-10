using ToolGood.Words;

namespace Berry.Spider.Core;

public static class StringExtensions
{
    private static readonly WordsMatch WordsMatch = new();

    static StringExtensions()
    {
        string dataFilePath = Path.Combine(AppContext.BaseDirectory, "SensitiveWords", "data.txt");
        if (File.Exists(dataFilePath))
        {
            List<string> sensitiveWords = File.ReadAllLines(dataFilePath)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToList();
            WordsMatch.SetKeywords(sensitiveWords);
        }
        else
        {
            WordsMatch.SetKeywords(new List<string>());
        }
    }

    /// <summary>
    /// 敏感词替换
    /// </summary>
    /// <param name="source">源字符串</param>
    /// <param name="replaceTo">替换成。默认*</param>
    /// <returns></returns>
    public static string ReplaceTo(this string source, char replaceTo = '*')
    {
        try
        {
            if (string.IsNullOrEmpty(source)) return source;
            string result = WordsMatch.Replace(source, replaceTo);
            return result;
        }
        catch (Exception e)
        {
            return source;
        }
    }
}