using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuPropertyFactoryMapper<TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<DestMember, Source, Dest, TService>(
            MapperConfigurationExpression cfg,
            YuzuPropertyFactoryMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<DestMember,TContext>;
    }

    public class DefaultPropertyFactoryMapper<TContext> : YuzuBaseMapper<YuzuPropertyFactoryMapperSettings>, IYuzuPropertyFactoryMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory<TContext> contextFactory;

        public DefaultPropertyFactoryMapper(IMappingContextFactory<TContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        // TODO: jiggle order to TSource, TDest, TMember, TService
        public void CreateMap<TMember, TSource, TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuPropertyFactoryMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<TMember, TContext>
        {
            config.AddActiveManualMap<TService, TDest>(settings.DestPropertyName);

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ForMember(settings.DestPropertyName, opt =>
            {
                opt.MapFrom( (_, _, _, context) =>
                {
                    var propertyResolver = factory.GetRequiredService<TService>();
                    var mappingContext = contextFactory.Create(context.Items);
                    return propertyResolver.Create(mappingContext);
                });
            });
        }

        protected override MethodInfo MakeGenericMethod(YuzuPropertyFactoryMapperSettings settings)
        {
            var genericArguments = settings.Factory.GetRelatedTypeParameters();
            genericArguments.Add(settings.Source);
            genericArguments.Add(settings.Dest);
            genericArguments.Add(settings.Factory);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
