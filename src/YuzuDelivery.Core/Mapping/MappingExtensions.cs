using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping;

internal static class MappingExtensions
{
    internal static void AddTypeAfterMap(this List<YuzuMapperSettings> resolvers, Type afterMapType)
    {
        resolvers.Add(new YuzuTypeAfterMapperSettings()
        {
            Mapper = typeof(IYuzuTypeAfterMapper<YuzuMappingContext>),
            Action = afterMapType
        });
    }
}
