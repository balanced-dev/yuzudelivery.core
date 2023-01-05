using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace YuzuDelivery.Core.Mapping;

public class DefaultTypeFactoryRunner<TContext> : IYuzuTypeFactoryRunner
    where TContext : YuzuMappingContext
{
    private readonly IOptions<YuzuConfiguration> _config;
    private readonly IMappingContextFactory<TContext> contextFactory;

    public DefaultTypeFactoryRunner(IOptions<YuzuConfiguration> config, IMappingContextFactory<TContext> contextFactory)
    {
        _config = config;
        this.contextFactory = contextFactory;
    }

    public TDest Run<TDest>(IDictionary<string, object> items = null)
    {
        if (!_config.Value.ViewmodelFactories.ContainsKey(typeof(TDest)))
        {
            return default;
        }

        var factory = _config.Value.ViewmodelFactories[typeof(TDest)]();

        // ReSharper disable once InvertIf
        if (factory is IYuzuTypeFactory<TDest, TContext> typeFactory)
        {
            items ??= new Dictionary<string, object>();
            return typeFactory.Create(contextFactory.Create(items));
        }

        return default;
    }
}
