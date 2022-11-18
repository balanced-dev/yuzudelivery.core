using System;
using AutoMapper;

namespace YuzuDelivery.Core.AutoMapper.Mappers;

public interface IYuzuBaseMapper
{
    void CreateMapAbstraction(
        MapperConfigurationExpression cfg,
        YuzuMapperSettings baseSettings,
        IServiceProvider factory,
        AddedMapContext mapContext,
        IYuzuConfiguration config);
}
