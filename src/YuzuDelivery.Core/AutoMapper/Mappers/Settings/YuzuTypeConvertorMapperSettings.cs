using System;

namespace YuzuDelivery.Core.AutoMapper.Mappers.Settings
{
    public class YuzuTypeConvertorMapperSettings : YuzuMapperSettings
    {
        public Type Convertor { get; set; }
        public bool IgnoreReturnType { get; set; }
    }
}
