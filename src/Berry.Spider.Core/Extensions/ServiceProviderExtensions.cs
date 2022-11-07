using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Berry.Spider.Core
{
    public static class ServiceProviderExtensions
    {
        private static readonly ConcurrentDictionary<SpiderSourceFrom, Type> Cache = new();

        static ServiceProviderExtensions()
        {
            //Assembly assembly = Assembly.Load("Berry.Spider.TouTiao", "Berry.Spider.Sogou", "Berry.Spider.Baidu");
            //foreach (Type type in assembly.GetTypes())
            //{
            //    SpiderAttribute? attribute = type.GetCustomAttribute<SpiderAttribute>();
            //    if (attribute != null)
            //    {
            //        Cache.TryAdd(attribute.SourceFrom, type);
            //    }
            //}
        }

        public static Type GetImplType(this IServiceProvider provider, SpiderSourceFrom from)
        {
            Type? implType = Cache.GetValueOrDefault(from);
            if (implType == null)
            {
                throw new NotImplementedException();
            }
            return implType;
        }
    }
}
