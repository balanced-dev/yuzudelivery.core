using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    public interface IMapper
    {
        object Concrete { get; }
        TDest Map<TDest>(object source);
        TDest Map<TSource, TDest>(TSource source);
        TDest Map<TDest>(object source, IDictionary<string, object> items);
        TDest Map<TSource, TDest>(TSource source, IDictionary<string, object> items);
        object Map(object source, Type sourceType, Type destinationType, IDictionary<string, object> items);

        IYuzuViewModel Map(object source, Type destinationType, IDictionary<string, object> items = null)
        {
            return Map(source, source.GetType(), destinationType, items ?? new Dictionary<string, object>()) as IYuzuViewModel;
        }
    }
}
