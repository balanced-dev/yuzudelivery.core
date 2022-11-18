using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuGlobalMapper : IYuzuBaseMapper
    {
        void CreateMap<TSource, TDest>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultGlobalMapper : IYuzuGlobalMapper
    {
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

        public void CreateMap<TSource, TDest>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
        {
            if (baseSettings is not YuzuGlobalMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuGlobalMapperSettings)}");
            }

            if (settings.GroupName != null)
            {
                cfg.RecognizePrefixes(settings.GroupName);
            }

            mapContext.AddOrGet<TSource, TDest>(cfg);
        }

        private MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuGlobalMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuGlobalMapperSettings)}");
            }

            var genericArguments = new List<Type>
            {
                settings.Source,
                settings.Dest
            };

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
