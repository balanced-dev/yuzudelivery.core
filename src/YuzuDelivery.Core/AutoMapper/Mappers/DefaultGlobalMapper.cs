using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;

namespace YuzuDelivery.Core.AutoMapper.Mappers
{
    public interface IYuzuGlobalMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Model, V>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultGlobalMapper : IYuzuGlobalMapper
    {
        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
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

            var method = GetType().GetMethod("CreateMap")!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

        public AddedMapContext CreateMap<Source, Dest>(
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

            mapContext.AddOrGet<Source, Dest>(cfg);
            return mapContext;
        }
    }
}
