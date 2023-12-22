using System.Collections.Concurrent;
using System.ComponentModel;
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
        return EnumDescriptionCache.GetOrAdd(enumType, () =>
        {
            // 获取枚举常数名称。
            string? name = Enum.GetName(enumType, source);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo? fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }

            return string.Empty;
        });
    }

    /// <summary>
    /// 获取枚举字符串名称
    /// </summary>
    /// <returns></returns>
    public static string GetName<T>(this T source)
    {
        Type enumType = typeof(T);
        return EnumNameCache.GetOrAdd(enumType, () => Enum.GetName(enumType, source));
    }
}