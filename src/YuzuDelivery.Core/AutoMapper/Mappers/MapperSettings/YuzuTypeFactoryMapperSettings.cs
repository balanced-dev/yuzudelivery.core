using System;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core
{
    public class YuzuTypeFactoryMapperSettings : YuzuMapperSettings
    {
        public Type Factory { get; set; }
        public Type Dest { get; set; }
    }
}
