using System.ComponentModel;
using System.Reflection;

namespace Berry.Spider.Core;

/// <summary>
/// 枚举相关扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举描述信息
    /// </summary>
    /// <returns></returns>
    public static string GetDescription(this Enum @enum)
    {
        Type enumType = @enum.GetType();
        // 获取枚举常数名称。
        string? name = Enum.GetName(enumType, @enum);
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
    }
}