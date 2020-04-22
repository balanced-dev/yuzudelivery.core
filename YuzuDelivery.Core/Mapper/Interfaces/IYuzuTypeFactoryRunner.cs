using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    public interface IYuzuTypeFactoryRunner
    {
        E Run<E>(IDictionary<string, object> items = null);
    }
}