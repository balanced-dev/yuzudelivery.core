using System;
using AutoMapper;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core
{
    public interface IYuzuTypeFactoryMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        AddedMapContext CreateMap<Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider serviceProvider, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<Dest, TContext>;
    }
}
