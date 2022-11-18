using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    public interface IYuzuTypeFactoryRunner
    {
        E Run<E>(IDictionary<string, object> items = null);
    }
}