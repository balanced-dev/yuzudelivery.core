using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuPropertyReplaceMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<M, PropertyType, V, TService>(
            MapperConfigurationExpression cfg,
            YuzuPropertyReplaceMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuPropertyReplaceResolver<M, PropertyType, TContext>;
    }

    public class DefaultPropertyReplaceMapper<TContext> : YuzuBaseMapper<YuzuPropertyReplaceMapperSettings>, IYuzuPropertyReplaceMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory<TContext> contextFactory;

        public DefaultPropertyReplaceMapper(IMappingContextFactory<TContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public void CreateMap<TSource, TDest, TMember, TResolver>(
            MapperConfigurationExpression cfg,
            YuzuPropertyReplaceMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TResolver : class, IYuzuPropertyReplaceResolver<TSource, TDest, TContext>
        {
            config.AddActiveManualMap<TResolver, TMember>(settings.DestPropertyName);

            if (!string.IsNullOrEmpty(settings.GroupName))
                cfg.RecognizePrefixes(settings.GroupName);

            var map = mapContext.AddOrGet<TSource, TMember>(cfg);

            map.ForMember(settings.DestPropertyName, opt =>
            {
                opt.MapFrom((TSource m, TMember v, object o, ResolutionContext context) =>
                {
                    var propertyResolver = factory.GetRequiredService<TResolver>();
                    var mappingContext = contextFactory.Create(context.Items);
                    return propertyResolver.Resolve(m, mappingContext);
                });
            });
        }

        protected override MethodInfo MakeGenericMethod(YuzuPropertyReplaceMapperSettings settings)
        {
            var genericArguments = settings.Resolver.GetRelatedTypeParameters();
            genericArguments.Add(settings.Dest);
            genericArguments.Add(settings.Resolver);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

    }
}
