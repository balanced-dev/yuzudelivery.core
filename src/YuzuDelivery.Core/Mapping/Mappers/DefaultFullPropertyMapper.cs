﻿using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuFullPropertyMapper<TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<Source, Destination, SourceMember, DestMember, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuFullPropertyResolver<Source, Destination, SourceMember, DestMember, TContext>;
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
        }

        private MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
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
    }
}