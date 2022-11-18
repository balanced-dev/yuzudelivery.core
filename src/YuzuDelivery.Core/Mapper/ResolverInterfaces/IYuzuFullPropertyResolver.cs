﻿using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuFullPropertyResolver { }

    public interface IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember, TContext>
        : IYuzuPropertyReplaceResolver, IYuzuMappingResolver
        where TContext : YuzuMappingContext
    {
        TDestMember Resolve(TSource source, TDest destination, TSourceMember sourceMember, string destPropertyName, TContext context);
    }
}
