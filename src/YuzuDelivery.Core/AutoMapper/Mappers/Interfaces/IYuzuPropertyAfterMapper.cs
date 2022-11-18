using System;
using AutoMapper;

namespace YuzuDelivery.Core
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
}
