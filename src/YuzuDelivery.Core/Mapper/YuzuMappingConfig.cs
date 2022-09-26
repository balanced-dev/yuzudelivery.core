using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    public class YuzuMappingConfig
    {
        public YuzuMappingConfig()
        {
            ManualMaps = new List<YuzuMapperSettings>();
        }

        public List<YuzuMapperSettings> ManualMaps { get; private set; }
        public virtual void AddMaps() { }
    }
}
