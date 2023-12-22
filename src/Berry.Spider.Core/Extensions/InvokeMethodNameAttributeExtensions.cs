using System.Collections.Concurrent;
using System.Reflection;

namespace Berry.Spider.Core;

public static class InvokeMethodNameAttributeExtensions
{
    private static readonly ConcurrentDictionary<Type, string> Cache = new();

    public static string GetMethodName(this Type type)
    {
        return Cache.GetOrAdd(type, () =>
        {
            if (type.IsDefined(typeof(InvokeMethodNameAttribute)))
            {
                var attr = type.GetCustomAttribute<InvokeMethodNameAttribute>();
                return attr?.MethodName ?? type.Name;
            }

            return type.Name;
        });
    }
}