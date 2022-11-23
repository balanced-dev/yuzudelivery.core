using System.Collections.Generic;
using AutoMapper;

namespace YuzuDelivery.Core.Mapping
{
    public interface IMappingContextFactory
    {
        T Create<T>(IDictionary<string, object> items) where T : YuzuMappingContext;

    }
}
