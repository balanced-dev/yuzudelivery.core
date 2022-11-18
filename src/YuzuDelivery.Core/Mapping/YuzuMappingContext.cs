using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    public class YuzuMappingContext
    {
        public IDictionary<string, object> Items { get; }

        public YuzuMappingContext(IDictionary<string, object> items)
        {
            Items = items;
        }
    }
}
