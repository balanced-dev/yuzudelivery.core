using AutoMapper;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace YuzuDelivery.Core
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultTypeAfterMapper<TContext> : IYuzuTypeAfterMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory _mappingContextFactory;

        public DefaultTypeAfterMapper(IMappingContextFactory mappingContextFactory)
        {
            _mappingContextFactory = mappingContextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings settings)
        {
            if (settings is not YuzuTypeAfterMapperSettings afterMapperSettings)
            {
                throw new Exception("Mapping settings not of type YuzuTypeAfterMapperSettings");
            }

            var genericArguments = afterMapperSettings.Action.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(afterMapperSettings.Action);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

        public AddedMapContext CreateMap<TSource, TDest, TConverter>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider serviceProvider,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TConverter : class, IYuzuTypeAfterConvertor<TSource, TDest, TContext>
        {
            config.AddActiveManualMap<TConverter, TDest>();

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.AfterMap((s, d, ctx) =>
            {
                var converter = serviceProvider.GetRequiredService<TConverter>();
                var mappingContext = _mappingContextFactory.Create<TContext>(ctx.Items);
                converter.Apply(s, d, mappingContext);
            });

            return mapContext;
        }
    }
}
