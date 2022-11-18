using System;
using AutoMapper;

namespace YuzuDelivery.Core
{
    public interface IYuzuTypeConvertorMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        AddedMapContext CreateMap<Source, Dest, TService>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeConvertor<Source, Dest, TContext>;
    }
}
