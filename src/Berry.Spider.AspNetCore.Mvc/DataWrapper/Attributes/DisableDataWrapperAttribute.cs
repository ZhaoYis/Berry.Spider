namespace Berry.Spider.AspNetCore.Mvc;

/// <summary>
/// 禁用数据包装特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableDataWrapperAttribute : Attribute
{

}