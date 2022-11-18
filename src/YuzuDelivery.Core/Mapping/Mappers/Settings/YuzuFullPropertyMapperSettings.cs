using System;

namespace YuzuDelivery.Core.Mapping.Mappers.Settings
{
    public class YuzuFullPropertyMapperSettings : YuzuMapperSettings
    {
        public Type Resolver { get; set; }
        public string SourcePropertyName { get; set; }
        public string DestPropertyName { get; set; }
        public string GroupName { get; set; }
        public bool IgnoreReturnType { get; set; }
        public bool IgnoreProperty { get; set; }
    }
}
