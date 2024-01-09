using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Berry.Spider.Core;

/// <summary>
/// 枚举相关扩展方法
/// </summary>
public static class EnumExtensions
{
    private static readonly ConcurrentDictionary<Type, string?> EnumDescriptionCache = new();
    private static readonly ConcurrentDictionary<Type, string?> EnumNameCache = new();

    /// <summary>
    /// 获取枚举描述信息
    /// </summary>
    /// <returns></returns>
    public static string GetDescription<T>(this T source)
    {
        Type enumType = typeof(T);
        string name = source.GetName();
        FieldInfo? fieldInfo = enumType.GetField(name);
        if (fieldInfo != null)
        {
            if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) is DescriptionAttribute attr)
            {
                return attr.Description;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 获取枚举字符串名称
    /// </summary>
    /// <returns></returns>
    public static string GetName<T>([NotNull] this T source)
    {
        return Enum.GetName(typeof(T), source);
    }
}