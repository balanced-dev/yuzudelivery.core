using System;
using AutoMapper;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuPropertyReplaceMapper<out TContext> : IYuzuBaseMapper
        where TContext : YuzuMappingContext
    {
        AddedMapContext CreateMap<M, PropertyType, V, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings settings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuPropertyReplaceResolver<M, PropertyType, TContext>;
    }
}
