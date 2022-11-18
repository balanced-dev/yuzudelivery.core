using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping;

public static class MappingExtensions
{
    public static void AddTypeAfterMap<TContext>(this List<YuzuMapperSettings> resolvers, Type afterMapType)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuTypeAfterMapperSettings()
        {
            Mapper = typeof(IYuzuTypeAfterMapper<TContext>),
            Action = afterMapType
        });
    }

    public static void AddTypeReplace<TContext>(this List<YuzuMapperSettings> resolvers, Type convertorType, bool ignoreReturnType = true)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuTypeConvertorMapperSettings()
        {
            Mapper = typeof(IYuzuTypeConvertorMapper<TContext>),
            Convertor = convertorType,
            IgnoreReturnType = ignoreReturnType
        });
    }

    public static void AddTypeFactory<TContext>(this List<YuzuMapperSettings> resolvers, Type factoryType, Type destType)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuTypeFactoryMapperSettings()
        {
            Mapper = typeof(IYuzuTypeFactoryMapper<TContext>),
            Factory = factoryType,
            Dest = destType,
        });
    }
}
