using Volo.Abp;

namespace Berry.Spider.Core;

/// <summary>
/// 高德地图全国区域编码
/// </summary>
public class AMapAdcodeOptions
{
    /// <summary>
    /// 高德地图全国区域编码
    /// </summary>
    public List<NameValue<string>> Items = new List<NameValue<string>>();
}