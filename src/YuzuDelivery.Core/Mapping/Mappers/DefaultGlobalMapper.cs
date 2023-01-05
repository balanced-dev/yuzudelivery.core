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
            YuzuGlobalMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            YuzuConfiguration config);
    }

    public class DefaultGlobalMapper : YuzuBaseMapper<YuzuGlobalMapperSettings>, IYuzuGlobalMapper
    {
        public virtual void CreateMap<TSource, TDest>(
            MapperConfigurationExpression cfg,
            YuzuGlobalMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            YuzuConfiguration config)
        {
            if (settings.GroupName != null)
            {
                cfg.RecognizePrefixes(settings.GroupName);
            }

            mapContext.AddOrGet<TSource, TDest>(cfg);
        }

        protected override MethodInfo MakeGenericMethod(YuzuGlobalMapperSettings settings)
        {
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
