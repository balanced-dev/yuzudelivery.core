using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;

namespace YuzuDelivery.Core.AutoMapper.Mappers
{
    public interface IYuzuPropertyReplaceMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        AddedMapContext CreateMap<M, PropertyType, V, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings settings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuPropertyReplaceResolver<M, PropertyType, TContext>;
    }

    public class DefaultPropertyReplaceMapper<TContext> : IYuzuPropertyReplaceMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory contextFactory;

        public DefaultPropertyReplaceMapper(IMappingContextFactory contextFatcory)
        {
            this.contextFactory = contextFatcory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuPropertyReplaceMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuPropertyReplaceMapperSettings)}");
            }

            var genericArguments = settings.Resolver.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(settings.Dest);
            genericArguments.Add(settings.Resolver);

            var method = GetType().GetMethod("CreateMap")!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

        public AddedMapContext CreateMap<TSource, TDest, TMember, TResolver>(MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TResolver : class, IYuzuPropertyReplaceResolver<TSource, TDest, TContext>
        {
            if (baseSettings is not YuzuPropertyReplaceMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuPropertyReplaceMapperSettings)}");
            }

            config.AddActiveManualMap<TResolver, TMember>(settings.DestPropertyName);

            if (!string.IsNullOrEmpty(settings.GroupName))
                cfg.RecognizePrefixes(settings.GroupName);

            var map = mapContext.AddOrGet<TSource, TMember>(cfg);

            map.ForMember(settings.DestPropertyName, opt =>
            {
                opt.MapFrom((TSource m, TMember v, object o, ResolutionContext context) =>
                {
                    var propertyResolver = factory.GetRequiredService<TResolver>();
                    var mappingContext = contextFactory.Create<TContext>(context.Items);
                    return propertyResolver.Resolve(m, mappingContext);
                });
            });

            return mapContext;
        }
    }
}
