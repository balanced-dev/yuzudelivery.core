using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;

namespace YuzuDelivery.Core.AutoMapper.Mappers
{
    public interface IYuzuTypeFactoryMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider serviceProvider, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<Dest, TContext>;
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultTypeFactoryMapper<TContext> : IYuzuTypeFactoryMapper<TContext>
        where TContext : YuzuMappingContext
    {
        public void CreateMapAbstraction(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
        {
            var method = MakeGenericMethod(baseSettings);
            method.Invoke(this, new object[] {cfg, baseSettings, factory, mapContext, config});
        }

        public void CreateMap<TDest, TService>(
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
        }

        private MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
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
    }
}
