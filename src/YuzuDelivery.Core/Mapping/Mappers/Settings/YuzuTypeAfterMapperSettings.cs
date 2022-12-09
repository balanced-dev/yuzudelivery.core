using System;

namespace YuzuDelivery.Core.Mapping.Mappers.Settings
{
    public class YuzuTypeAfterMapperSettings : YuzuMapperSettings
    {
        public Type Action { get; set; }

        public bool ApplyToDerivedTypes { get; set; }
    }
}
