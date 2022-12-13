using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers;

namespace YuzuDelivery.Core.Mapping;

public class MapperBuilder
{
    public IServiceCollection Services { get; private set; }
    public YuzuConfiguration YuzuConfig { get; private set; }

    private readonly YuzuMappingConfig _defaultMappingConfig;

    private Action<IYuzuConfiguration, AutoMapper.MapperConfigurationExpression, AddedMapContext> _action;


    public MapperBuilder()
    {
        YuzuConfig = new YuzuConfiguration(Enumerable.Empty<IUpdateableConfig>());
        Services = new ServiceCollection();

        _defaultMappingConfig = new YuzuMappingConfig();
        _action = (_, _, _) => { };
    }

    public MapperBuilder WithServiceCollection(IServiceCollection services)
    {
        Services = services;
        return this;
    }

    public MapperBuilder WithYuzuConfiguration(YuzuConfiguration yuzuConfig)
    {
        YuzuConfig = yuzuConfig;
        return this;
    }


    public MapperBuilder WithConfigureAction(
        Action<IYuzuConfiguration, AutoMapper.MapperConfigurationExpression, AddedMapContext> action)
    {
        _action = action;
        return this;
    }

    public MapperBuilder AddMappingConfig<TConfig>()
        where TConfig : YuzuMappingConfig
    {
        Services.AddSingleton<YuzuMappingConfig, TConfig>();
        return this;
    }

    public MapperBuilder AddManualMapper(Action<List<YuzuMapperSettings>> cfg)
    {
        cfg(_defaultMappingConfig.ManualMaps);
        return this;
    }

    private IServiceProvider _container;

    public IYuzuTypeFactoryRunner TypeFactoryRunner
    {
        get
        {
            if (_container == null)
            {
                throw new InvalidOperationException("Call Build first");
            }

            return _container.GetRequiredService<IYuzuTypeFactoryRunner>();
        }
    }

    public IMapper Build()
    {
        // Default mapper implementations
        Services.AddSingleton<IYuzuGroupMapper, DefaultGroupMapper>()
                .AddSingleton<IYuzuGlobalMapper, DefaultGlobalMapper>()
                .AddSingleton<IYuzuFullPropertyMapper<TestMappingContext>, DefaultFullPropertyMapper<TestMappingContext>>()
                .AddSingleton<IYuzuPropertyAfterMapper, DefaultPropertyAfterMapper>()
                .AddSingleton<IYuzuPropertyFactoryMapper<TestMappingContext>, DefaultPropertyFactoryMapper<TestMappingContext>>()
                .AddSingleton<IYuzuPropertyReplaceMapper<TestMappingContext>, DefaultPropertyReplaceMapper<TestMappingContext>>()
                .AddSingleton<IYuzuTypeAfterMapper<TestMappingContext>, DefaultTypeAfterMapper<TestMappingContext>>()
                .AddSingleton<IYuzuTypeReplaceMapper<TestMappingContext>, DefaultTypeReplaceMapper<TestMappingContext>>()
                .AddSingleton<IYuzuTypeFactoryMapper<TestMappingContext>, DefaultTypeFactoryMapper<TestMappingContext>>();

        Services.AddSingleton<IYuzuTypeFactoryRunner, DefaultTypeFactoryRunner<TestMappingContext>>();

        Services.AddSingleton<IMappingContextFactory<TestMappingContext>, TestContextFactory>();
        Services.AddSingleton(_defaultMappingConfig);

        Services.AddSingleton<IYuzuConfiguration>(YuzuConfig);

        // Register all discovered downstream converters
        Services.RegisterYuzuManualMapping(Assembly.GetExecutingAssembly());

        _container = Services.BuildServiceProvider();
        var factory = new DefaultYuzuMapperFactory(YuzuConfig, _container);

        return factory.Create(_action);
    }

    class TestContextFactory : IMappingContextFactory<TestMappingContext>
    {
        public TestMappingContext Create(IDictionary<string, object> items)
        {
            return new TestMappingContext(items);
        }
    }
}
