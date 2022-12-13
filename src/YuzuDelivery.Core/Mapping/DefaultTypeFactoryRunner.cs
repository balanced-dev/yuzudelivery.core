using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping;

public class DefaultTypeFactoryRunner<TContext> : IYuzuTypeFactoryRunner
    where TContext : YuzuMappingContext
{
    private readonly IYuzuConfiguration config;
    private readonly IMappingContextFactory<TContext> contextFactory;

    public DefaultTypeFactoryRunner(IYuzuConfiguration config, IMappingContextFactory<TContext> contextFactory)
    {
        this.config = config;
        this.contextFactory = contextFactory;
    }

    public TDest Run<TDest>(IDictionary<string, object> items = null)
    {
        if (!config.ViewmodelFactories.ContainsKey(typeof(TDest)))
        {
            return default;
        }

        var factory = config.ViewmodelFactories[typeof(TDest)]();

        // ReSharper disable once InvertIf
        if (factory is IYuzuTypeFactory<TDest, TContext> typeFactory)
        {
            items ??= new Dictionary<string, object>();
            return typeFactory.Create(contextFactory.Create(items));
        }

        return default;
    }
}
