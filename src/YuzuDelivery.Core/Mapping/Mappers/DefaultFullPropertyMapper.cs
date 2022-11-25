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
            YuzuFullPropertyMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration _)
            where TService : class, IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember, TContext>;
    }

    public class DefaultFullPropertyMapper<TContext> : YuzuBaseMapper<YuzuFullPropertyMapperSettings>, IYuzuFullPropertyMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory<TContext> _contextFactory;

        public DefaultFullPropertyMapper(IMappingContextFactory<TContext> contextFactory) =>
            _contextFactory = contextFactory;

        public void CreateMap<TSource, TDest, TSourceMember, TDestMember, TService>(
            MapperConfigurationExpression cfg,
            YuzuFullPropertyMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember, TContext>
        {
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
                    var mappingContext = _contextFactory.Create(ctx.Items);

                    return propertyResolver.Resolve(src, dest, sourceValue, settings.DestPropertyName, mappingContext);
                });
            });
        }

        protected override MethodInfo MakeGenericMethod(YuzuFullPropertyMapperSettings settings)
        {
            var genericArguments = settings.Resolver.GetRelatedTypeParameters();
            genericArguments.Add(settings.Resolver);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
