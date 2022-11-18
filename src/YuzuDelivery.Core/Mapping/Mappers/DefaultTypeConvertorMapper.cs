﻿using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuTypeConvertorMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        void CreateMap<Source, Dest, TService>(
            MapperConfigurationExpression cfg,
            YuzuTypeConvertorMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeConvertor<Source, Dest, TContext>;
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultTypeConvertorMapper<TContext> : YuzuBaseMapper<YuzuTypeConvertorMapperSettings>, IYuzuTypeConvertorMapper<TContext>
        where TContext : YuzuMappingContext
    {
        private readonly IMappingContextFactory contextFactory;

        public DefaultTypeConvertorMapper(IMappingContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public void CreateMap<TSource, TDest, TConverter>(
            MapperConfigurationExpression cfg,
            YuzuTypeConvertorMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TConverter : class, IYuzuTypeConvertor<TSource, TDest, TContext>
        {
            config.AddActiveManualMap<TConverter, TDest>();

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ConvertUsing((src, _, context) =>
            {
                var typeConvertor = factory.GetRequiredService<TConverter>();
                var mappingContext = contextFactory.Create<TContext>(context.Items);

                return typeConvertor.Convert(src, mappingContext);
            });
        }

        protected override MethodInfo MakeGenericMethod(YuzuTypeConvertorMapperSettings settings)
        {
            var genericArguments = settings.Convertor.GetInterfaces().First().GetGenericArguments().ToList();
            genericArguments.Add(settings.Convertor);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
