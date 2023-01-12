using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterAll<T>(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(T)));

            foreach (var f in types)
            {
                services.AddTransient(typeof(T), f);
            }
        }

        public static void RegisterYuzuManualMapping(this IServiceCollection services, Assembly profileAssembly)
        {
            var types = profileAssembly.GetTypes();
            var allowedInterfaces = new Type[] { typeof(IYuzuTypeAfterConvertor), typeof(IYuzuTypeConvertor), typeof(IYuzuTypeFactory), typeof(IYuzuPropertyAfterResolver), typeof(IYuzuPropertyReplaceResolver), typeof(IYuzuFullPropertyResolver) };

            foreach (var i in types.Where(x => allowedInterfaces.Intersect(x.GetInterfaces()).Any()))
            {
                if (i.GetInterfaces().Any(x => x == typeof(IYuzuTypeFactory)))
                    services.AddSingleton(typeof(IYuzuTypeFactory), i);

                services.AddSingleton(i);
            }
        }
    }
}
