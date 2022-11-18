using System;
using System.Reflection;
using AutoMapper;

namespace YuzuDelivery.Core.Mapping.Mappers;

public abstract class YuzuBaseMapper<TSettings> : IYuzuBaseMapper
    where TSettings : YuzuMapperSettings
{
    public void CreateMapAbstraction(
        MapperConfigurationExpression cfg,
        YuzuMapperSettings baseSettings,
        IServiceProvider factory,
        AddedMapContext mapContext,
        IYuzuConfiguration config)
    {
        if (baseSettings is not TSettings settings)
        {
            throw new Exception($"Mapping settings not of type {typeof(TSettings).Name}");
        }

        var method = MakeGenericMethod(settings);
        method.Invoke(this, new object[] {cfg, settings, factory, mapContext, config});
    }

    protected abstract MethodInfo MakeGenericMethod(TSettings settings);
}
