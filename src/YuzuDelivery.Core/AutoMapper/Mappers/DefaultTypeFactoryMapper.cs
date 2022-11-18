using AutoMapper;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace YuzuDelivery.Core
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultTypeFactoryMapper<TContext> : IYuzuTypeFactoryMapper<TContext>
        where TContext : YuzuMappingContext
    {
        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuTypeFactoryMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuTypeFactoryMapperSettings)}");
            }

            var genericArguments = settings.Factory.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(settings.Factory);

            var method = GetType().GetMethod("CreateMap")!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

        public AddedMapContext CreateMap<TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider serviceProvider,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<TDest, TContext>
        {
            if (baseSettings is not YuzuTypeFactoryMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuTypeFactoryMapperSettings)}");
            }

            if (!config.ViewmodelFactories.ContainsKey(settings.Dest))
            {
                config.ViewmodelFactories.Add(settings.Dest, () => serviceProvider.GetRequiredService(typeof(TService)) as TService);
            }

            config.AddActiveManualMap<TService, TDest>();

            return mapContext;
        }
    }
}
