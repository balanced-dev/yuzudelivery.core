using System;

namespace YuzuDelivery.Core.AutoMapper.Mappers.Settings
{
    public class YuzuPropertyAfterMapperSettings : YuzuMapperSettings
    {
        public Type Resolver { get; set; }
        public Type Dest { get; set; }
        public object DestProperty { get; set; }
        public string GroupName { get; set; }
    }
}
