using System;
using System.Collections.Generic;
using System.Linq;

namespace YuzuDelivery.Core.Mapping;

public static class TypeExtensions
{
    public static List<Type> GetRelatedTypeParameters(this Type t)
    {
        if (t == null)
        {
            throw new ArgumentNullException(nameof(t));
        }

        return t.GetInterfaces()
                .First()
                .GetGenericArguments()
                .Where(x => !x.IsSubclassOf(typeof(YuzuMappingContext)))
                .ToList();
    }
}
