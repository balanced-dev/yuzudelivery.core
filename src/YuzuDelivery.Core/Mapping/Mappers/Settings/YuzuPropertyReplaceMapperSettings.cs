using System;

namespace YuzuDelivery.Core.Mapping.Mappers.Settings
{
    public class YuzuPropertyReplaceMapperSettings : YuzuMapperSettings
    {
        public Type Resolver { get; set; }
        public Type Dest { get; set; }
        public string DestPropertyName { get; set; }
        public string GroupName { get; set; }
        public bool IgnoreReturnType { get; set; }
        public bool IgnoreProperty { get; set; }
    }
}
