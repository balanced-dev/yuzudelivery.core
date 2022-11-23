using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuTypeAfterMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<Source, Dest, TService>(
            MapperConfigurationExpression cfg,
            YuzuTypeAfterMapperSettings baseSettings,
            IServiceProvider serviceProvider,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeAfterConvertor<Source, Dest, TContext>;
    }

    public class DefaultTypeAfterMapper<TContext> : YuzuBaseMapper<YuzuTypeAfterMapperSettings>, IYuzuTypeAfterMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory _mappingContextFactory;

        public DefaultTypeAfterMapper(IMappingContextFactory mappingContextFactory)
            => _mappingContextFactory = mappingContextFactory;

        public void CreateMap<TSource, TDest, TConverter>(
            MapperConfigurationExpression cfg,
            YuzuTypeAfterMapperSettings settings,
            IServiceProvider serviceProvider,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TConverter : class, IYuzuTypeAfterConvertor<TSource, TDest, TContext>
        {
            config.AddActiveManualMap<TConverter, TDest>();

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.AfterMap((src, dest, ctx) =>
            {
                var converter = serviceProvider.GetRequiredService<TConverter>();
                var mappingContext = _mappingContextFactory.Create<TContext>(ctx.Items);
                converter.Apply(src, dest, mappingContext);
            });
        }

        protected override MethodInfo MakeGenericMethod(YuzuTypeAfterMapperSettings settings)
        {
            var genericArguments = settings.Action.GetRelatedTypeParameters();
            genericArguments.Add(settings.Action);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
