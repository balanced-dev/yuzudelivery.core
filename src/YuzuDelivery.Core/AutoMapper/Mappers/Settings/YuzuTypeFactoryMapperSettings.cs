using System;

namespace YuzuDelivery.Core.AutoMapper.Mappers.Settings
{
    public class YuzuTypeFactoryMapperSettings : YuzuMapperSettings
    {
        public Type Factory { get; set; }
        public Type Dest { get; set; }
    }
}
