using System;
using AutoMapper;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core
{
    public interface IYuzuGlobalMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Model, V>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config);
    }
}
