using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    public interface IMappingContextFactory
    {
        T Create<T>(IDictionary<string, object> items) where T : YuzuMappingContext;
    }
}
