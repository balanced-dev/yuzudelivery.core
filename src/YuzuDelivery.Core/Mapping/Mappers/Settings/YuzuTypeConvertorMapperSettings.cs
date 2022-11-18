using System;

namespace YuzuDelivery.Core.Mapping.Mappers.Settings
{
    public class YuzuTypeConvertorMapperSettings : YuzuMapperSettings
    {
        public Type Convertor { get; set; }
        public bool IgnoreReturnType { get; set; }
    }
}
