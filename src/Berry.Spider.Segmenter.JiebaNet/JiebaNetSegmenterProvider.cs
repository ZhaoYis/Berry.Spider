using JiebaNet.Segmenter;
using Volo.Abp;

namespace Berry.Spider.Segmenter.JiebaNet;

/// <summary>
/// jieba.net分词
/// </summary>
public class JiebaNetSegmenterProvider : ISegmenterProvider
{
    /**
     *  var segmenter = new JiebaSegmenter();

        var segments = segmenter.Cut("我来到北京清华大学", cutAll: true);
        Console.WriteLine("【全模式】：{0}", string.Join("/ ", segments));
        【全模式】：我/ 来到/ 北京/ 清华/ 清华大学/ 华大/ 大学

        segments = segmenter.Cut("我来到北京清华大学");  // 默认为精确模式
        Console.WriteLine("【精确模式】：{0}", string.Join("/ ", segments));
        【精确模式】：我/ 来到/ 北京/ 清华大学

        segments = segmenter.Cut("他来到了网易杭研大厦");  // 默认为精确模式，同时也使用HMM模型
        Console.WriteLine("【新词识别】：{0}", string.Join("/ ", segments));
        【新词识别】：他/ 来到/ 了/ 网易/ 杭研/ 大厦

        segments = segmenter.CutForSearch("小明硕士毕业于中国科学院计算所，后在日本京都大学深造"); // 搜索引擎模式
        Console.WriteLine("【搜索引擎模式】：{0}", string.Join("/ ", segments));【搜索引擎模式】：小明/ 硕士/ 毕业/ 于/ 中国/ 科学/ 学院/ 科学院/ 中国科学院/ 计算/ 计算所/ ，/ 后/ 在/ 日本/ 京都/ 大学/ 日本京都大学/ 深造
        
        segments = segmenter.Cut("结过婚的和尚未结过婚的");
        Console.WriteLine("【歧义消除】：{0}", string.Join("/ ", segments));
        【歧义消除】：结过婚/ 的/ 和/ 尚未/ 结过婚/ 的
     */
    
    /// <summary>
    /// 中文分词
    /// </summary>
    /// <returns></returns>
    public Task<List<string>> CutForSearchAsync(string source)
    {
        Check.NotNull(source, nameof(source));

        var segmenter = new JiebaSegmenter();
        //搜索引擎模式
        var segments = segmenter.CutForSearch(source).ToList();

        return Task.FromResult(segments);
    }
}