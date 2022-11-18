using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;

namespace YuzuDelivery.Core.AutoMapper.Mappers
{
    public interface IYuzuPropertyAfterMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<M, PropertyType, V, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings settings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuPropertyAfterResolver<M, PropertyType>;
    }

    public interface IYuzuPropertyAfterResolver<M, Type> : IYuzuPropertyAfterResolver, IYuzuMappingResolver
    {
        Type Apply(Type value);
    }

    public class DefaultPropertyAfterMapper : IYuzuPropertyAfterMapper
    {
        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuPropertyAfterMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuPropertyAfterMapperSettings)}");
            }

            var genericArguments = settings.Resolver.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(settings.Dest);
            genericArguments.Add(settings.Resolver);

            var method = GetType().GetMethod("CreateMap")!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

        public AddedMapContext CreateMap<TSource, TMember, TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuPropertyAfterResolver<TSource, TMember>
        {
            if (baseSettings is not YuzuPropertyAfterMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuPropertyAfterMapperSettings)}");
            }

            //need a fix here
            //config.AddActiveManualMap<Resolver, Dest>(settings.DestProperty);

            if (!string.IsNullOrEmpty(settings.GroupName))
            {
                cfg.RecognizePrefixes(settings.GroupName);
            }

            Func<TMember, TMember> mappingFunction = (input) =>
            {
                var propertyResolver = factory.GetRequiredService<TService>();
                return propertyResolver.Apply(input);
            };

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ForMember(settings.DestProperty as Expression<Func<TDest, TMember>>, opt => {
                opt.AddTransform(x => mappingFunction(x));
            });

            return mapContext;
        }
    }
}
