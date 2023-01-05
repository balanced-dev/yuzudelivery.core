using System;
using AutoMapper;

namespace YuzuDelivery.Core.Mapping.Mappers;

public interface IYuzuBaseMapper
{
    void CreateMapAbstraction(
        MapperConfigurationExpression cfg,
        YuzuMapperSettings baseSettings,
        IServiceProvider factory,
        AddedMapContext mapContext,
        YuzuConfiguration config);
}
