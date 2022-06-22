namespace Berry.Spider.Core;

public class FormattingTitleProvider : IFormattingTitleProvider
{
    private const string DefaultFormatTemplate = "{0}精选{1}句";
    
    /// <summary>
    /// 按照配置规则格式化标题
    /// </summary>
    /// <returns></returns>
    public string Format(string title, int total)
    {
        if (total <= 0) return title;
        
        return string.Format(DefaultFormatTemplate, title, total);
    }
}