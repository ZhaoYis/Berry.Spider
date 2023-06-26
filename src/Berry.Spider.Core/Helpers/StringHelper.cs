namespace Berry.Spider.Core;

public static class StringHelper
{
    /// <summary>
    /// 余弦相似度
    /// https://blog.csdn.net/u012160689/article/details/15341303
    /// </summary>
    /// <returns></returns>
    public static double Sim(string txt1, string txt2)
    {
        /**
         *  其中，A、B分别是文本一、文本二对应的n维向量，取值方式用语言比较难描述，直接看例子吧：
            例：文本一是“一个雨伞”，文本二是“下雨了开雨伞”，计算它们的余弦相似度。
            
            它们的并集是{一，个，雨，伞，下，了，开}，共7个字。
            若并集中的第1个字符在文本一中出现了n次，则A1=n（n=0，1，2……）。
            若并集中的第2个字符在文本一中出现了n次，则A2=n（n=0，1，2……）。
            依此类推，算出A3、A4、……、A7，B1、B2、……、B7，最终得到：
            A=（1，1，1，1，0，0，0）。
            B=（0，0，2，1，1，1，1）。
            https://img-blog.csdn.net/20131111190818687
         * 
         */

        List<char> sl1 = txt1.ToCharArray().ToList();
        List<char> sl2 = txt2.ToCharArray().ToList();
        //去重
        List<char> sl = sl1.Union(sl2).ToList<char>();

        //获取重复次数
        List<int> arrA = new List<int>();
        List<int> arrB = new List<int>();
        foreach (var str in sl)
        {
            arrA.Add(sl1.Count(x => x == str));
            arrB.Add(sl2.Count(x => x == str));
        }

        //计算商
        double num = 0;
        //被除数
        double numA = 0;
        double numB = 0;
        for (int i = 0; i < sl.Count; i++)
        {
            //分子
            num += arrA[i] * arrB[i];

            //分母
            numA += Math.Pow(arrA[i], 2);
            numB += Math.Pow(arrB[i], 2);
        }

        double cos = num / (Math.Sqrt(numA) * Math.Sqrt(numB));
        return cos;
    }
}