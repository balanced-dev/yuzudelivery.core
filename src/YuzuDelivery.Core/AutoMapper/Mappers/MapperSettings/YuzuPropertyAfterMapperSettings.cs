using System;
using YuzuDelivery.Core;
using System.Linq.Expressions;

namespace YuzuDelivery.Umbraco.Core
{
    public class YuzuPropertyAfterMapperSettings : YuzuMapperSettings
    {
        public Type Resolver { get; set; }
        public Type Dest { get; set; }
        public object DestProperty { get; set; }
        public string GroupName { get; set; }
    }
}
