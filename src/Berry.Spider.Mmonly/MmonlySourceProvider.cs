using Berry.Spider.Mmonly.Contracts;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Mmonly;

public class MmonlySourceProvider : IMmonlySourceProvider,ISingletonDependency
{
    public IEnumerable<string> GetUrls()
    {
        //帅哥
        for (int i = 1; i <= 965; i++)
        {
            yield return $"https://www.mmonly.cc/sgtp/list_1_{i}.html";
        }
        //唯美
        for (int i = 1; i <= 1297; i++)
        {
            yield return $"https://www.mmonly.cc/wmtp/list_20_{i}.html";
        }
        //卡通
        for (int i = 1; i <= 446; i++)
        {
            yield return $"https://www.mmonly.cc/ktmh/list_28_{i}.html";
        }
        //高清壁纸
        for (int i = 1; i <= 444; i++)
        {
            yield return $"https://www.mmonly.cc/gqbz/list_41_{i}.html";
        }
        //美女图片
        for (int i = 1; i <= 1344; i++)
        {
            yield return $"https://www.mmonly.cc/mmtp/list_9_{i}.html";
        }
        //美女图片
        for (int i = 1; i <= 479; i++)
        {
            yield return $"https://www.mmonly.cc/mmtp/xgmn/list_10_{i}.html";
        }
        //内衣美女
        for (int i = 1; i <= 93; i++)
        {
            yield return $"https://www.mmonly.cc/mmtp/nymn/list_15_{i}.html";
        }
        //内衣美女
        for (int i = 1; i <= 193; i++)
        {
            yield return $"https://www.mmonly.cc/mmtp/mnmx/list_18_{i}.html";
        }
        //内衣美女
        for (int i = 1; i <= 71; i++)
        {
            yield return $"https://www.mmonly.cc/mmtp/ctmn/list_17_{i}.html";
        }
        //其他图片
        for (int i = 1; i <= 1158; i++)
        {
            yield return $"https://www.mmonly.cc/qttp/list_54_{i}.html";
        }
    }
}