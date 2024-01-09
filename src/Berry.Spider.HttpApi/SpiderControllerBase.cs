using Berry.Spider.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 自定义控制器基类
/// </summary>
[DisableDataWrapper]
public class SpiderControllerBase : AbpControllerBase
{
}