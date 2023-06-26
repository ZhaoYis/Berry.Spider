using DotNetCore.CAP;
using System.Reflection;

namespace Berry.Spider.EventBus.RabbitMq;

public static class CapSubscribeExtensions
{
    public static CapSubscribeAttribute? GetAttribute<T>(this T t)
    {
        if (t == null) throw new ArgumentNullException(nameof(t));

        Type type = t.GetType();
        CapSubscribeAttribute? attribute = type.GetCustomAttribute<CapSubscribeAttribute>();
        if (attribute != null)
        {
            return attribute;
        }

        return default;
    }

    public static string TryGetTopicName<T>(this T t)
    {
        CapSubscribeAttribute? attribute = GetAttribute(t);
        if (attribute == null)
        {
            return nameof(T);
        }
        else
        {
            return attribute.Name ?? nameof(T);
        }
    }
}