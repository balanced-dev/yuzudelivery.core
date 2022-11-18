using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    public interface IMapper
    {
        object Concrete { get; }
        E Map<E>(object source);
        E Map<E>(object source, IDictionary<string, object> items);
        object Map(object source, Type sourceType, Type destinationType, IDictionary<string, object> items);
    }
}
