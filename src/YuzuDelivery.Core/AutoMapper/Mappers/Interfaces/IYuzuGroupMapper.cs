using System;
using AutoMapper;

namespace YuzuDelivery.Core
{
    public interface IYuzuGroupMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Model, VParent, VChild>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config);
    }
}
