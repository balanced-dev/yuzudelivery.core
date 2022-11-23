using System;
using System.Collections.Generic;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping;

public static class MappingExtensions
{
    public static void AddTypeAfterMapWithContext<TContext>(this List<YuzuMapperSettings> resolvers, Type afterMapType)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuTypeAfterMapperSettings()
        {
            Mapper = typeof(IYuzuTypeAfterMapper<TContext>),
            Action = afterMapType
        });
    }

    public static void AddTypeReplaceWithContext<TContext, ConvertorType>(this List<YuzuMapperSettings> resolvers, bool ignoreReturnType = true)
        where TContext : YuzuMappingContext
        where ConvertorType : IYuzuTypeConvertor
    {
        resolvers.AddTypeReplaceWithContext<TContext>(typeof(ConvertorType), ignoreReturnType);
    }

    public static void AddTypeReplaceWithContext<TContext>(
        this List<YuzuMapperSettings> resolvers,
        Type convertorType,
        bool ignoreReturnType = true)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuTypeConvertorMapperSettings()
        {
            Mapper = typeof(IYuzuTypeReplaceMapper<TContext>),
            Convertor = convertorType,
            IgnoreReturnType = ignoreReturnType
        });
    }

    public static void AddTypeFactoryWithContext<TContext, TFactory, TDest>(this List<YuzuMapperSettings> resolvers)
        where TContext : YuzuMappingContext
        where TFactory : IYuzuTypeFactory
    {
        resolvers.AddTypeFactoryWithContext<TContext>(typeof(TFactory), typeof(TDest));
    }

    public static void AddTypeFactoryWithContext<TContext>(
        this List<YuzuMapperSettings> resolvers,
        Type factoryType,
        Type destType)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuTypeFactoryMapperSettings()
        {
            Mapper = typeof(IYuzuTypeFactoryMapper<TContext>),
            Factory = factoryType,
            Dest = destType,
        });
    }

    public static void AddPropertyReplaceWithContext<TContext>(
        this List<YuzuMapperSettings> resolvers,
        Type resolverType,
        Type destType,
        string destMemberName,
        string groupName = "",
        bool ignoreProperty = true,
        bool ignoreReturnType = true)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuPropertyReplaceMapperSettings()
        {
            Mapper = typeof(IYuzuPropertyReplaceMapper<TContext>),
            Resolver = resolverType,
            Dest = destType,
            DestPropertyName = destMemberName,
            GroupName = groupName,
            IgnoreProperty = ignoreProperty,
            IgnoreReturnType = ignoreReturnType
        });
    }

    public static void AddPropertyFactoryWithContext<TContext>(
        this List<YuzuMapperSettings> resolvers,
        Type factoryType,
        Type sourceType,
        Type destType,
        string destMemberName)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuPropertyFactoryMapperSettings()
        {
            Mapper = typeof(IYuzuPropertyFactoryMapper<TContext>),
            Factory = factoryType,
            Source = sourceType,
            Dest = destType,
            DestPropertyName = destMemberName
        });
    }

    public static void AddFullPropertyWithContext<TContext>(
        this List<YuzuMapperSettings> resolvers,
        Type resolverType,
        string sourceMemberName,
        string destMemberName,
        string groupName = "",
        bool ignoreProperty = true,
        bool ignoreReturnType = true)
        where TContext : YuzuMappingContext
    {
        resolvers.Add(new YuzuFullPropertyMapperSettings()
        {
            Mapper = typeof(IYuzuFullPropertyMapper<TContext>),
            Resolver = resolverType,
            SourcePropertyName = sourceMemberName,
            DestPropertyName = destMemberName,
            GroupName = groupName,
            IgnoreProperty = ignoreProperty,
            IgnoreReturnType = ignoreReturnType
        });
    }


}
