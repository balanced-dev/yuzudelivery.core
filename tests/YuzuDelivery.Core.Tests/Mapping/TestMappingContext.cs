using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping;

public class TestMappingContext : YuzuMappingContext
{
    public TestMappingContext(IDictionary<string, object> items)
        : base(items)
    {
    }
}
