using System.Reflection;

namespace Berry.Spider.Core;

/// <summary>
/// 枚举基类
/// </summary>
/// <param name="value">枚举值</param>
/// <param name="name">自定义名称</param>
public abstract class EnumerationBase(int value, string name)
{
    public int Value { get; private set; } = value;
    public string Name { get; private set; } = name;

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : EnumerationBase =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj is not EnumerationBase otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Value.Equals(otherValue.Value);

        return typeMatches && valueMatches;
    }

    public int CompareTo(object other) => Value.CompareTo(((EnumerationBase)other).Value);
}