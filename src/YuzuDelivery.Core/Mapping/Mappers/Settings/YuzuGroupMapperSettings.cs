using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping.Mappers.Settings
{
    public class YuzuGroupMapperSettings : YuzuMapperSettings
    {
        public Type Source { get; set; }
        public Type DestParent { get; set; }
        public Type DestChild { get; set; }
        public string PropertyName { get; set; }
        public string GroupName { get; set; }
        public IEnumerable<string> SourceProperties { get; set; }
    }
}
