using System.Collections.Concurrent;
using System.Reflection;
using Volo.Abp.EventBus;

namespace Berry.Spider.Core;

public static class SpiderEventNameAttributeExtensions
{
    private static readonly ConcurrentDictionary<Type, SpiderEventNameAttribute?> EtoTypeCache = new();
    private static readonly ConcurrentDictionary<SpiderSourceFrom, List<SpiderEventNameAttribute>?> EtoTypesCache = new();

    static SpiderEventNameAttributeExtensions()
    {
        List<Type> exportedTypes = new List<Type>();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies.Where(c => c.FullName != null && c.FullName.StartsWith("Berry")))
        {
            exportedTypes.AddRange(assembly.ExportedTypes);
        }

        foreach (Type type in exportedTypes)
        {
            SpiderEventNameAttribute? attribute = type.GetCustomAttribute<SpiderEventNameAttribute>();
            if (attribute != null)
            {
                if (EtoTypesCache.ContainsKey(attribute.SourceFrom))
                {
                    var list = EtoTypesCache[attribute.SourceFrom];
                    list?.Add(attribute);

                    EtoTypesCache[attribute.SourceFrom] = list;
                }
                else
                {
                    EtoTypesCache.TryAdd(attribute.SourceFrom, new List<SpiderEventNameAttribute> { attribute });
                }
            }
        }
    }

    public static string TryGetRoutingKey<T>(this T t)
    {
        SpiderEventNameAttribute? attribute = GetAttribute(t);
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
        List<SpiderEventNameAttribute>? attributes = GetAttributes(from);
        if (attributes is { Count: > 0 })
        {
            return attributes.Select(c => c.Name).ToList();
        }
        else
        {
            return new List<string>();
        }
    }

    private static SpiderEventNameAttribute? GetAttribute<T>(this T t)
    {
        if (t == null) throw new ArgumentNullException(nameof(t));

        Type type = t.GetType();
        return EtoTypeCache.GetOrAdd(type, t =>
        {
            SpiderEventNameAttribute? attribute = t.GetCustomAttribute<SpiderEventNameAttribute>();
            if (attribute != null)
            {
                return attribute;
            }

            return default;
        });
    }

    private static List<SpiderEventNameAttribute>? GetAttributes(this SpiderSourceFrom from)
    {
        return EtoTypesCache[from];
    }
}