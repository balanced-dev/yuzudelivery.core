using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Umbraco.Blocks
{
    public class RenderSettings : IRenderSettings
    {
        public string Template { get; set; }
        public object MapFrom { get; set; }
        public Func<object> Data { get; set; }
        public bool ShowJson { get; set; }
        public string CacheName { get; set; }
        public int CacheExpiry { get; set; }
    } 
}
