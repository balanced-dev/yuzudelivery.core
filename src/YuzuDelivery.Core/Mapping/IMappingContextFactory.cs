using System.Collections.Generic;
using AutoMapper;

namespace YuzuDelivery.Core.Mapping
{
    public interface IMappingContextFactory<out TContext>
        where TContext : YuzuMappingContext
    {
        TContext Create(IDictionary<string, object> items);
    }
}
