using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;

namespace YuzuDelivery.Core.AutoMapper.Mappers
{
    public interface IYuzuPropertyFactoryMapper<TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<DestMember, Source, Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<DestMember,TContext>;
    }

    public class DefaultPropertyFactoryMapper<TContext> : IYuzuPropertyFactoryMapper<TContext>
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

        private readonly IMappingContextFactory contextFactory;

        public DefaultPropertyFactoryMapper(IMappingContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }


        // TODO: jiggle order to TSource, TDest, TMember, TService
        public void CreateMap<TMember, TSource, TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<TMember, TContext>
        {
            if (baseSettings is not YuzuPropertyFactoryMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuPropertyFactoryMapperSettings)}");
            }

            config.AddActiveManualMap<TService, TDest>(settings.DestPropertyName);

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ForMember(settings.DestPropertyName, opt =>
            {
                opt.MapFrom( (_, _, _, context) =>
                {
                    var propertyResolver = factory.GetRequiredService<TService>();
                    var mappingContext = contextFactory.Create<TContext>(context.Items);
                    return propertyResolver.Create(mappingContext);
                });
            });
        }

        private MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuPropertyFactoryMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuPropertyFactoryMapperSettings)}");
            }

            var genericArguments = settings.Factory.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(settings.Source);
            genericArguments.Add(settings.Dest);
            genericArguments.Add(settings.Factory);

            var method = GetType().GetMethod("CreateMap")!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

    }
}
