using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    public class MappingContextFactory : IMappingContextFactory
    {
        public virtual T Create<T>(IDictionary<string, object> items)
            where T : YuzuMappingContext
        {
            return new YuzuMappingContext(items) as T;
        }
    }
}
