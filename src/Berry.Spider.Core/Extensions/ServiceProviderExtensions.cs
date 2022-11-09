using System.Collections.Concurrent;
using System.Reflection;

namespace Berry.Spider.Core
{
    public static class ServiceProviderExtensions
    {
        private static readonly ConcurrentDictionary<SpiderSourceFrom, Type> Cache = new();

        static ServiceProviderExtensions()
        {
            List<Type> exportedTypes = new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies.Where(c => c.FullName != null && c.FullName.StartsWith("Berry")))
            {
                exportedTypes.AddRange(assembly.ExportedTypes);
            }

            foreach (Type type in exportedTypes)
            {
                SpiderAttribute? attribute = type.GetCustomAttribute<SpiderAttribute>();
                if (attribute != null)
                {
                    Cache.TryAdd(attribute.SourceFrom, type);
                }
            }
        }

        public static object GetImplService(this IServiceProvider provider, SpiderSourceFrom from)
        {
            Type? implType = Cache.GetValueOrDefault(from);
            if (implType == null)
            {
                throw new NotImplementedException();
            }

            object? o = provider.GetService(implType);
            if (o == null)
            {
                throw new NotImplementedException();
            }

            return o;
        }
    }
}