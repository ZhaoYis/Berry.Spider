using System.Reflection;
using Volo.Abp.EventBus;

namespace Berry.Spider.Core;

public static class EventNameAttributeExtensions
{
    public static string TryGetEventName<T>(this T t)
    {
        EventNameAttribute? attribute = GetAttribute(t);
        if (attribute == null)
        {
            return nameof(T);
        }
        else
        {
            return attribute.Name ?? nameof(T);
        }
    }

    private static EventNameAttribute? GetAttribute<T>(this T t)
    {
        if (t == null) throw new ArgumentNullException(nameof(t));

        Type type = t.GetType();
        EventNameAttribute? attribute = type.GetCustomAttribute<EventNameAttribute>();
        if (attribute != null)
        {
            return attribute;
        }

        return default;
    }
}