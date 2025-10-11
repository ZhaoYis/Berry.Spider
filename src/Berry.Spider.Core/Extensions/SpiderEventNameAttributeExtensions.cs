using System.Collections.Concurrent;
using System.Reflection;

namespace Berry.Spider.Core;

public static class SpiderEventNameAttributeExtensions
{
    private static readonly ConcurrentDictionary<Type, SpiderEventNameAttribute?> EventNameAttributeCache = new();
    private static readonly ConcurrentDictionary<SpiderSourceFrom, List<EventNameCacheItem>?> EventNameAttributesCache = new();

    static SpiderEventNameAttributeExtensions()
    {
        List<Type> exportedTypes = AssemblyHelper.CurrentDomainExportedTypes;

        foreach (Type type in exportedTypes)
        {
            SpiderEventNameAttribute? eventNameAttribute = type.GetCustomAttribute<SpiderEventNameAttribute>();
            if (eventNameAttribute != null)
            {
                if (EventNameAttributesCache.ContainsKey(eventNameAttribute.SourceFrom))
                {
                    var list = EventNameAttributesCache[eventNameAttribute.SourceFrom] ?? [];
                    list.Add(new EventNameCacheItem(type, eventNameAttribute));

                    EventNameAttributesCache[eventNameAttribute.SourceFrom] = list;
                }
                else
                {
                    EventNameAttributesCache.TryAdd(eventNameAttribute.SourceFrom, [new EventNameCacheItem(type, eventNameAttribute)]);
                }
            }
        }
    }

    public static string TryGetRoutingKey<T>(this T t)
    {
        SpiderEventNameAttribute? attribute = t.GetAttribute();
        if (attribute == null)
        {
            return nameof(T);
        }
        else
        {
            return attribute.Name ?? nameof(T);
        }
    }

    public static List<string> TryGetRoutingKeys(this SpiderSourceFrom from)
    {
        List<SpiderEventNameAttribute>? attributes = from.GetAttributes();
        if (attributes is { Count: > 0 })
        {
            return attributes.Select(c => c.Name).ToList();
        }
        else
        {
            return [];
        }
    }

    public static object TryCreateEto(this SpiderSourceFrom from, EtoType type, params object?[]? args)
    {
        Type? etoObjType = from.GetEtoType(type);
        if (etoObjType is not null)
        {
            object? instance = Activator.CreateInstance(etoObjType, args);
            return instance ?? new object();
        }

        throw new NotSupportedException($"Cannot create Eto object of type {type}");
    }

    private static SpiderEventNameAttribute? GetAttribute<T>(this T t)
    {
        if (t == null) throw new ArgumentNullException(nameof(t));

        Type type = t.GetType();
        return EventNameAttributeCache.GetOrAdd(type, t =>
        {
            SpiderEventNameAttribute? attribute = t.GetCustomAttribute<SpiderEventNameAttribute>();
            return attribute ?? null;
        });
    }

    private static List<SpiderEventNameAttribute>? GetAttributes(this SpiderSourceFrom from)
    {
        if (EventNameAttributesCache.TryGetValue(from, out List<EventNameCacheItem>? value))
        {
            if (value != null) return value.Select(e => e.EventNameAttribute).ToList();
        }

        return null;
    }

    private static Type? GetEtoType(this SpiderSourceFrom from, EtoType type)
    {
        if (EventNameAttributesCache.TryGetValue(from, out List<EventNameCacheItem>? value))
        {
            if (value is { Count: > 0 })
            {
                return value.Where(e => e.EventNameAttribute.EtoType == type)
                    .Select(e => e.ObjType)
                    .FirstOrDefault();
            }
        }

        return null;
    }
}

internal class EventNameCacheItem
{
    public EventNameCacheItem(Type objType, SpiderEventNameAttribute eventNameAttribute)
    {
        this.ObjType = objType;
        this.EventNameAttribute = eventNameAttribute;
    }

    public Type ObjType { get; set; }

    public SpiderEventNameAttribute EventNameAttribute { get; set; }
}