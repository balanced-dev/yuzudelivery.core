using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuFullPropertyMapper<TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<TSource, TDest, TSourceMember, TDestMember, TService>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember, TContext>;
    }

    public class DefaultFullPropertyMapper<TContext> : IYuzuFullPropertyMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory contextFactory;

        public DefaultFullPropertyMapper(IMappingContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

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

        public void CreateMap<TSource, TDest, TSourceMember, TDestMember, TService>(
            MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory,
            AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember, TContext>
        {
            if (baseSettings is not YuzuFullPropertyMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuFullPropertyMapperSettings)}");
            }

            if (!string.IsNullOrEmpty(settings.GroupName))
            {
                cfg.RecognizePrefixes(settings.GroupName);
            }

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ForMember(settings.DestPropertyName, opt =>
            {
                opt.MapFrom((src, dest, _, ctx) =>
                {
                    var propertyResolver = factory.GetRequiredService<TService>();
                    var sourceValue = ((TSourceMember) typeof(TSource).GetProperty(settings.SourcePropertyName)!.GetValue(src));
                    var mappingContext = contextFactory.Create<TContext>(ctx.Items);

                    return propertyResolver.Resolve(src, dest, sourceValue, settings.DestPropertyName, mappingContext);
                });
            });
        }

        private MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuFullPropertyMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuFullPropertyMapperSettings)}");
            }

            var genericArguments = settings.Resolver.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(settings.Resolver);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
