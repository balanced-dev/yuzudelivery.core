using System;

namespace YuzuDelivery.Umbraco.Blocks
{
    public interface IRenderSettings
    {
        int CacheExpiry { get; set; }
        string CacheName { get; set; }
        Func<object> Data { get; set; }
        object MapFrom { get; set; }
        bool ShowJson { get; set; }
        string Template { get; set; }
    }
}