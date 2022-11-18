using System;

namespace YuzuDelivery.Core.Mapping.Mappers.Settings
{
    public class YuzuGlobalMapperSettings : YuzuMapperSettings
    {
        public Type Source { get; set; }
        public Type Dest { get; set; }
        public string GroupName { get; set; }
    }
}
