using System.Collections.Immutable;
using ToolGood.Words;

namespace Berry.Spider.Core;

public static class StringExtensions
{
    private static readonly WordsMatch WordsMatch = new();

    static StringExtensions()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "SensitiveWords");
        var files = Directory.GetFiles(filePath, "*.txt", SearchOption.AllDirectories);

        ImmutableList<string> sensitiveWords = ImmutableList.Create<string>();
        foreach (string file in files)
        {
            List<string> words = File.ReadAllLines(file)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();

            sensitiveWords = sensitiveWords.AddRange(words);
        }

        WordsMatch.SetKeywords(sensitiveWords);
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