using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultFullPropertyMapper<TContext> : IYuzuFullPropertyMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory contextFactory;

        public DefaultFullPropertyMapper(IMappingContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuFullPropertyMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuFullPropertyMapperSettings)}");
            }

            var genericArguments = settings.Resolver.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(settings.Resolver);

            var method = GetType().GetMethod("CreateMap")!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

        public AddedMapContext CreateMap<TSource, TDest, TSourceMember, TDestMember, TService>(
            MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory,
            AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember, TContext>
        {
            if (baseSettings is not YuzuFullPropertyMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuFullPropertyMapperSettings)}");
            }


            if (!string.IsNullOrEmpty(settings.GroupName))
                cfg.RecognizePrefixes(settings.GroupName);

            Func<TSource, TDest, object, ResolutionContext, TDestMember> mappingFunction =
                (TSource m, TDest v, object o, ResolutionContext context) =>
                {
                    var propertyResolver = factory.GetService(typeof(TService)) as TService;
                    var sourceValue =
                        ((TSourceMember) typeof(TSource).GetProperty(settings.SourcePropertyName).GetValue(m));
                    var yuzuContext = contextFactory.Create<TContext>(context.Items);

                    return propertyResolver.Resolve(m, v, sourceValue, settings.DestPropertyName, yuzuContext);
                };

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ForMember(settings.DestPropertyName, opt => opt.MapFrom(mappingFunction));

            return mapContext;
        }
    }
}
