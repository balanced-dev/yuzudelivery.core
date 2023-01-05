using System;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuTypeFactoryMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<Dest, TService>(MapperConfigurationExpression cfg, YuzuTypeFactoryMapperSettings baseSettings, IServiceProvider serviceProvider, AddedMapContext mapContext, YuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<Dest, TContext>;
    }

    public class DefaultTypeFactoryMapper<TContext> : YuzuBaseMapper<YuzuTypeFactoryMapperSettings>, IYuzuTypeFactoryMapper<TContext>
        where TContext : YuzuMappingContext
    {
        public void CreateMap<TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuTypeFactoryMapperSettings settings,
            IServiceProvider serviceProvider,
            AddedMapContext mapContext,
            YuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<TDest, TContext>
        {

            if (!config.ViewmodelFactories.ContainsKey(settings.Dest))
            {
                config.ViewmodelFactories.Add(settings.Dest, serviceProvider.GetRequiredService<TService>);
            }

            config.AddActiveManualMap<TService, TDest>();
        }

        protected override MethodInfo MakeGenericMethod(YuzuTypeFactoryMapperSettings settings)
        {
            var genericArguments = settings.Factory.GetRelatedTypeParameters();
            genericArguments.Add(settings.Factory);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
