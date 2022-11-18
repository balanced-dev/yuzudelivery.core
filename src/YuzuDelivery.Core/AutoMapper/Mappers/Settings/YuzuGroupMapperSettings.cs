using System;

namespace YuzuDelivery.Core.AutoMapper.Mappers.Settings
{
    public class YuzuGroupMapperSettings : YuzuMapperSettings
    {
        public Type Source { get; set; }
        public Type DestParent { get; set; }
        public Type DestChild { get; set; }
        public string PropertyName { get; set; }
        public string GroupName { get; set; }
    }
}
