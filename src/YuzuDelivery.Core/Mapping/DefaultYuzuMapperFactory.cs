using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers;

namespace YuzuDelivery.Core.Mapping;

public class DefaultYuzuMapperFactory
{
    private readonly IYuzuConfiguration _yuzuConfig;
    private readonly IServiceProvider _serviceProvider;

    private AddedMapContext _addedMapContext;

    public DefaultYuzuMapperFactory(IYuzuConfiguration yuzuConfig, IServiceProvider serviceProvider)
    {
        _yuzuConfig = yuzuConfig;
        _serviceProvider = serviceProvider;
        _addedMapContext = new AddedMapContext();
    }

    public IMapper Create(Action<IYuzuConfiguration, global::AutoMapper.MapperConfigurationExpression, AddedMapContext> configure)
    {
        var cfg = new global::AutoMapper.MapperConfigurationExpression();
        cfg.ConstructServicesUsing(_serviceProvider.GetService);

        AddYuzuMappersFromContainer(cfg);
        AddProfilesFromContainer(cfg);

        configure(_yuzuConfig, cfg, _addedMapContext);

        var config = new global::AutoMapper.MapperConfiguration(cfg);

        var info = typeof(AutoMapper.MapperConfiguration).GetField("_resolvedMaps", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(config);
        if (info is Dictionary<AutoMapper.Internal.TypePair, AutoMapper.TypeMap> resolvedMaps)
        {
            for (var i = resolvedMaps.Keys.Count - 1; i > 0; i--)
            {
                var key = resolvedMaps.Keys.ElementAt(i);
                if (resolvedMaps[key] == null)
                {
                    resolvedMaps.Remove(key);
                }
            }
            resolvedMaps.TrimExcess();
        }

        return new DefaultYuzuMapper(config.CreateMapper());
    }

    private void AddProfilesFromContainer(global::AutoMapper.MapperConfigurationExpression cfg)
    {
        var loadedProfiles = GetProfiles(_yuzuConfig.MappingAssemblies);

        foreach (var profile in loadedProfiles)
        {
            var resolvedProfile = _serviceProvider.GetService(profile) as global::AutoMapper.Profile;
            cfg.AddProfile(resolvedProfile);
        }
    }

    private void AddYuzuMappersFromContainer(global::AutoMapper.MapperConfigurationExpression cfg)
    {
        var mappingConfigs = _serviceProvider.GetServices<YuzuMappingConfig>();
        var config = _serviceProvider.GetService<IYuzuConfiguration>();

        foreach (var mappingConfig in mappingConfigs)
        {
            foreach (var item in mappingConfig.ManualMaps)
            {
                if (_serviceProvider.GetService(item.Mapper) is not IYuzuBaseMapper mapper)
                {
                    continue;
                }

                mapper.CreateMapAbstraction(cfg, item, _serviceProvider, _addedMapContext, _yuzuConfig);
            }
        }
    }

    private static List<Type> GetProfiles(IEnumerable<Assembly> assemblies)
    {
        var profiles = new List<Type>();
        foreach (var assembly in assemblies)
        {
            var assemblyProfiles = assembly.ExportedTypes.Where(type => type.IsSubclassOf(typeof(global::AutoMapper.Profile)));
            profiles.AddRange(assemblyProfiles);
        }
        return profiles;
    }
}
