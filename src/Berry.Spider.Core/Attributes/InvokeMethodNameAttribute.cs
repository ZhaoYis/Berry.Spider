namespace Berry.Spider.Core;

[AttributeUsage(AttributeTargets.Class)]
public class InvokeMethodNameAttribute : Attribute
{
    public InvokeMethodNameAttribute(string methodName)
    {
        MethodName = methodName;
    }

    /// <summary>
    /// 方法名称
    /// </summary>
    public string MethodName { get; set; }
}