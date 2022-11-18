using System.Collections.Generic;

namespace YuzuDelivery.Core.AutoMapper
{
    public interface IMappingContextFactory
    {
        T Create<T>(IDictionary<string, object> items) where T : YuzuMappingContext;
    }
}
