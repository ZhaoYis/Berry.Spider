using System.Reflection;

namespace Berry.Spider.Core;

public static class InvokeMethodNameAttributeExtensions
{
    public static string GetMethodName(this Type type)
    {
        if (type.IsDefined(typeof(InvokeMethodNameAttribute)))
        {
            var attr = type.GetCustomAttribute<InvokeMethodNameAttribute>();
            return attr?.MethodName ?? type.Name;
        }

        return type.Name;
    }
}