using System.Collections.Concurrent;
using System.Reflection;

namespace Berry.Spider.Core
{
    public static class ServiceProviderExtensions
    {
        private static readonly ConcurrentDictionary<SpiderSourceFrom, Type> Cache = new();

        static ServiceProviderExtensions()
        {
            List<Type> exportedTypes = AssemblyHelper.CurrentDomainExportedTypes;

            foreach (Type type in exportedTypes)
            {
                SpiderServiceAttribute? attribute = type.GetCustomAttribute<SpiderServiceAttribute>();
                if (attribute is {SourceFromArray: {Length: > 0}})
                {
                    foreach (SpiderSourceFrom sourceFrom in attribute.SourceFromArray)
                    {
                        Cache.TryAdd(sourceFrom, type);
                    }
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