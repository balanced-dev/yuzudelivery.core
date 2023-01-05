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
        void CreateMap<TSource, TMember, TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuPropertyReplaceMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            YuzuConfiguration config)
            where TService : class, IYuzuPropertyReplaceResolver<TSource, TMember, TContext>;
    }

    public class DefaultPropertyReplaceMapper<TContext> : YuzuBaseMapper<YuzuPropertyReplaceMapperSettings>, IYuzuPropertyReplaceMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory<TContext> contextFactory;

        public DefaultPropertyReplaceMapper(IMappingContextFactory<TContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public void CreateMap<TSource, TMember, TDest, TResolver>(
            MapperConfigurationExpression cfg,
            YuzuPropertyReplaceMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            YuzuConfiguration config)
            where TResolver : class, IYuzuPropertyReplaceResolver<TSource, TMember, TContext>
        {
            config.AddActiveManualMap<TResolver, TDest>(settings.DestPropertyName);

            if (!string.IsNullOrEmpty(settings.GroupName))
                cfg.RecognizePrefixes(settings.GroupName);

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ForMember(settings.DestPropertyName, opt =>
            {
                opt.MapFrom((source, dest, member, context) =>
                {
                    var propertyResolver = factory.GetRequiredService<TResolver>();
                    var mappingContext = contextFactory.Create(context.Items);
                    return propertyResolver.Resolve(source, mappingContext);
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
