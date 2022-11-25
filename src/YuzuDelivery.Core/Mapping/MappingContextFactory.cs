using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    public class MappingContextFactory : IMappingContextFactory<YuzuMappingContext>
    {
        public YuzuMappingContext Create(IDictionary<string, object> items)
        {
            return new YuzuMappingContext(items);
        }
    }
}
