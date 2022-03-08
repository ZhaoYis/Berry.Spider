namespace Berry.Spider.Core;

public static class AsciiCodeExtensions
{
    /**
     * 中文范围：4E00-9FA5
     */

    /**
     * 中文符号
     */
    private static List<string> ChineseSymbolUnicode = new List<string>
    {
        /*句号 。 */ "3002",
        /*问号 ？ */ "FF1F",
        /*叹号 ！ */ "FF01",
        /*逗号 ， */ "FF0C",
        /*顿号 、 */ "3001",
        /*分号 ； */ "FF1B",
        /*冒号 ： */ "FF1A",
        /*引号 「 */ "300C",
        /*--- 」 */ "300D",
        /*引号 『 */ "300E",
        /*--- 』 */ "300F",
        /*引号 ‘ */ "2018",
        /*--- ’ */ "2019",
        /*引号 “ */ "201C",
        /*--- ” */ "201D",
        /*括号 （ */ "FF08",
        /*---  ）*/ "FF09",
        /*括号 〔 */ "3014",
        /*--- 〕 */ "3015",
        /*括号 【 */ "3010",
        /*--- 】 */ "3011",
        /*破折号 — */ "2014",
        /*省略号 … */ "2026",
        /*连接号 – */ "2013",
        /*间隔号 ．*/ "FF0E",
        /*书名号 《 */ "300A",
        /*---  》 */ "300B",
        /*书名号 〈 */ "3008",
        /*--- 〉 */ "3009"
    };

    /// <summary>
    /// 检查是否是中文符号
    /// https://www.jianshu.com/p/a1a9a98c7bd9
    /// </summary>
    /// <returns></returns>
    public static bool IsChineseSymbol(this char code)
    {
        List<int> list = ChineseSymbolUnicode.Select(c => Convert.ToInt32(c, 16)).ToList();

        return list.Contains(code);
    }
}