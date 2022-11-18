using System;

namespace YuzuDelivery.Core.Mapping.Mappers.Settings
{
    public class YuzuPropertyFactoryMapperSettings : YuzuMapperSettings
    {
        public Type Factory { get; set; }
        public Type Source { get; set; }
        public Type Dest { get; set; }
        public string DestPropertyName { get; set; }
        public string GroupName { get; set; }
    }
}
