using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Settings;

namespace YuzuDelivery.Core.Mapping;

public class DefaultYuzuMapperFactory
{
    private readonly IOptions<YuzuConfiguration> _yuzuConfig;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DefaultYuzuMapperFactory> _logger;

    private AddedMapContext _addedMapContext;

    public DefaultYuzuMapperFactory(IOptions<YuzuConfiguration> yuzuConfig, IServiceProvider serviceProvider, ILogger<DefaultYuzuMapperFactory> logger)
    {
        _yuzuConfig = yuzuConfig;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _addedMapContext = new AddedMapContext();
    }

    public IMapper Create(Action<YuzuConfiguration, global::AutoMapper.MapperConfigurationExpression, AddedMapContext> configure)
    {
        try
        {
            var cfg = new global::AutoMapper.MapperConfigurationExpression();
            cfg.ConstructServicesUsing(_serviceProvider.GetService);

            AddYuzuMappersFromContainer(cfg);
            AddProfilesFromContainer(cfg);

            configure(_yuzuConfig.Value, cfg, _addedMapContext);

            var config = new global::AutoMapper.MapperConfiguration(cfg);

            return new DefaultYuzuMapper(config.CreateMapper());
        }
        catch (Exception ex)
        {
            // HACK: We have some registrations which cause issues on first boot.
            // TODO: Investigate YuzuBlockListStartup etc (essentially all type replace mappers).
            _logger.LogError(ex, "Failed to configure IMapper.");
            return new BadConfigurationYuzuMapper(ex);
        }
    }

    private void AddProfilesFromContainer(global::AutoMapper.MapperConfigurationExpression cfg)
    {
        var loadedProfiles = GetProfiles(_yuzuConfig.Value.MappingAssemblies);

        foreach (var profile in loadedProfiles)
        {
            var resolvedProfile = _serviceProvider.GetService(profile) as global::AutoMapper.Profile;
            cfg.AddProfile(resolvedProfile);
        }
    }

    private void AddYuzuMappersFromContainer(global::AutoMapper.MapperConfigurationExpression cfg)
    {
        var mappingConfigs = _serviceProvider.GetServices<IOptions<ManualMapping>>();


        foreach (var mappingConfig in mappingConfigs)
        {
            foreach (var item in mappingConfig.Value.ManualMaps)
            {
                if (_serviceProvider.GetService(item.Mapper) is not IYuzuBaseMapper mapper)
                {
                    continue;
                }

                mapper.CreateMapAbstraction(cfg, item, _serviceProvider, _addedMapContext, _yuzuConfig.Value);
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
