using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Settings;

namespace YuzuDelivery.Core.Mapping;

public class MapperBuilder
{
    public IServiceCollection Services { get; private set; }
    public YuzuConfiguration YuzuConfig { get; private set; }

    private readonly IOptions<ManualMapping> _defaultMappingConfig;

    private Action<YuzuConfiguration, AutoMapper.MapperConfigurationExpression, AddedMapContext> _action;


    public MapperBuilder()
    {
        YuzuConfig = new YuzuConfiguration();
        Services = new ServiceCollection();

        _defaultMappingConfig = Options.Create(new ManualMapping());
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
        Action<YuzuConfiguration, AutoMapper.MapperConfigurationExpression, AddedMapContext> action)
    {
        _action = action;
        return this;
    }



    public MapperBuilder AddManualMapper(Action<IList<YuzuMapperSettings>> cfg)
    {
        cfg(_defaultMappingConfig.Value.ManualMaps);
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
                .AddSingleton<IYuzuTypeAfterMapper<TestMappingContext>, DefaultTypeAfterMapper<TestMappingContext>>()
                .AddSingleton<IYuzuTypeReplaceMapper<TestMappingContext>, DefaultTypeReplaceMapper<TestMappingContext>>()
                .AddSingleton<IYuzuTypeFactoryMapper<TestMappingContext>, DefaultTypeFactoryMapper<TestMappingContext>>();

        Services.AddSingleton<IYuzuTypeFactoryRunner, DefaultTypeFactoryRunner<TestMappingContext>>();

        Services.AddSingleton<IMappingContextFactory<TestMappingContext>, TestContextFactory>();
        Services.AddSingleton(_defaultMappingConfig);

        Services.AddOptions<YuzuConfiguration>();

        // Register all discovered downstream converters
        Services.RegisterYuzuManualMapping(Assembly.GetExecutingAssembly());

        _container = Services.BuildServiceProvider();
        var factory = new DefaultYuzuMapperFactory(_container.GetRequiredService<IOptions<YuzuConfiguration>>(), _container);

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
