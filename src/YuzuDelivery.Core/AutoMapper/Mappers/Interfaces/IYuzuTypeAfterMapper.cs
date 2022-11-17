using System;
using AutoMapper;

namespace YuzuDelivery.Core
{
    public interface IYuzuTypeAfterMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        AddedMapContext CreateMap<Source, Dest, TService>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider serviceProvider,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuTypeAfterConvertor<Source, Dest, TContext>;
    }
}
