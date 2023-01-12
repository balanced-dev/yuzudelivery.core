using System.Collections.Generic;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Core.Settings
{
    public class ManualMapping
    {
        public IList<YuzuMapperSettings> ManualMaps { get; } = new List<YuzuMapperSettings>();
    }
}
